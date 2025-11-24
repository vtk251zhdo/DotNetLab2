const columns = document.querySelectorAll('.column');
const columnBodies = document.querySelectorAll('.column-body');
const addTaskBtn = document.getElementById('add-task-btn');
const modal = document.getElementById('task-modal');
const form = document.getElementById('task-form');
const deleteBtn = document.getElementById('delete-task');
const cancelBtn = document.getElementById('cancel-modal');
const closeModalBtn = document.getElementById('close-modal');
const searchInput = document.getElementById('search');
const statusFilter = document.getElementById('filter-status');
const priorityFilter = document.getElementById('filter-priority');
const template = document.getElementById('task-template');
const counts = {
  backlog: document.getElementById('count-backlog'),
  todo: document.getElementById('count-todo'),
  'in-progress': document.getElementById('count-in-progress'),
  review: document.getElementById('count-review'),
  done: document.getElementById('count-done')
};
const stats = {
  total: document.getElementById('stat-total'),
  progress: document.getElementById('stat-progress'),
  done: document.getElementById('stat-done'),
  deadline: document.getElementById('stat-deadline')
};

const STATUS_LABELS = {
  backlog: 'Backlog',
  todo: 'To Do',
  'in-progress': 'In Progress',
  review: 'Review',
  done: 'Done'
};

const PRIORITY_COLORS = {
  high: 'var(--red)',
  medium: 'var(--amber)',
  low: 'var(--green)'
};

let tasks = loadTasks();

if (!tasks.length) {
  tasks = createSeedData();
  saveTasks();
}

renderBoard();

addTaskBtn.addEventListener('click', () => openModal());
closeModalBtn.addEventListener('click', () => modal.close());
cancelBtn.addEventListener('click', () => modal.close());

form.addEventListener('submit', (event) => {
  event.preventDefault();
  const data = new FormData(form);
  const id = data.get('id') || crypto.randomUUID();
  const title = data.get('title').trim();
  const description = data.get('description').trim();
  const assignee = data.get('assignee').trim();
  const priority = data.get('priority');
  const status = data.get('status');
  const dueDate = data.get('dueDate');

  if (!title) return;

  const existing = tasks.find((task) => task.id === id);
  const now = new Date().toISOString();
  if (existing) {
    Object.assign(existing, {
      title,
      description,
      assignee,
      priority,
      status,
      dueDate,
      updatedAt: now
    });
  } else {
    const order = getNextOrder(status);
    tasks.push({
      id,
      title,
      description,
      assignee,
      priority,
      status,
      dueDate,
      createdAt: now,
      updatedAt: now,
      order
    });
  }

  saveTasks();
  renderBoard();
  modal.close();
});

deleteBtn.addEventListener('click', () => {
  const id = form.querySelector('#task-id').value;
  if (!id) return modal.close();
  tasks = tasks.filter((task) => task.id !== id);
  saveTasks();
  renderBoard();
  modal.close();
});

searchInput.addEventListener('input', renderBoard);
statusFilter.addEventListener('change', renderBoard);
priorityFilter.addEventListener('change', renderBoard);

columnBodies.forEach((body) => {
  body.addEventListener('dragover', handleDragOver);
  body.addEventListener('dragleave', () => body.classList.remove('drag-over'));
  body.addEventListener('drop', handleDrop);
});

function renderBoard() {
  const term = searchInput.value.toLowerCase();
  const statusValue = statusFilter.value;
  const priorityValue = priorityFilter.value;

  const filtered = tasks
    .filter((task) =>
      statusValue === 'all' ? true : task.status === statusValue
    )
    .filter((task) =>
      priorityValue === 'all' ? true : task.priority === priorityValue
    )
    .filter((task) =>
      !term
        ? true
        : [task.title, task.description, task.assignee]
            .filter(Boolean)
            .some((field) => field.toLowerCase().includes(term))
    )
    .sort((a, b) => a.order - b.order);

  columnBodies.forEach((body) => {
    body.innerHTML = '';
    const status = body.parentElement.dataset.status;
    const tasksInColumn = filtered.filter((task) => task.status === status);

    if (!tasksInColumn.length) {
      const empty = document.createElement('div');
      empty.className = 'empty-state';
      empty.textContent = 'No tasks here yet';
      body.append(empty);
    } else {
      tasksInColumn.forEach((task) => body.append(createCard(task)));
    }
  });

  updateCounts(filtered);
  updateStats(filtered);
}

function createCard(task) {
  const card = template.content.firstElementChild.cloneNode(true);
  card.dataset.id = task.id;
  card.querySelector('.title').textContent = task.title;
  card.querySelector('.assignee').textContent = task.assignee || 'Unassigned';
  card.querySelector('.badge').style.background = PRIORITY_COLORS[task.priority];
  card.querySelector('.description').textContent = task.description || 'No description';
  const dueLabel = card.querySelector('.due');
  if (task.dueDate) {
    const dueDate = new Date(task.dueDate);
    dueLabel.textContent = `Due ${dueDate.toLocaleDateString()}`;
    if (isOverdue(task)) {
      dueLabel.style.background = '#fff1f2';
      dueLabel.style.color = 'var(--red)';
    }
  } else {
    dueLabel.textContent = 'No due date';
  }

  card.querySelector('.status').textContent = STATUS_LABELS[task.status];

  card.addEventListener('click', (event) => {
    if (event.target.classList.contains('icon-btn')) return;
    openModal(task);
  });

  card.addEventListener('dragstart', (event) => {
    card.setAttribute('aria-grabbed', 'true');
    event.dataTransfer.setData('text/plain', task.id);
    event.dataTransfer.effectAllowed = 'move';
  });

  card.addEventListener('dragend', () => card.removeAttribute('aria-grabbed'));

  return card;
}

function handleDragOver(event) {
  event.preventDefault();
  const body = event.currentTarget;
  const after = getDragAfterElement(body, event.clientY);
  body.classList.add('drag-over');
  const dragging = document.querySelector('.card[aria-grabbed="true"]');
  if (!dragging) return;
  if (after == null) {
    body.appendChild(dragging);
  } else {
    body.insertBefore(dragging, after);
  }
}

function handleDrop(event) {
  event.preventDefault();
  const body = event.currentTarget;
  body.classList.remove('drag-over');
  const id = event.dataTransfer.getData('text/plain');
  const task = tasks.find((item) => item.id === id);
  if (!task) return;
  const newStatus = body.parentElement.dataset.status;
  task.status = newStatus;
  reorderFromDom();
  saveTasks();
  renderBoard();
}

function getDragAfterElement(container, y) {
  const draggableElements = [...container.querySelectorAll('.card:not([aria-grabbed="true"])')];
  return draggableElements.reduce(
    (closest, child) => {
      const box = child.getBoundingClientRect();
      const offset = y - box.top - box.height / 2;
      if (offset < 0 && offset > closest.offset) {
        return { offset, element: child };
      }
      return closest;
    },
    { offset: Number.NEGATIVE_INFINITY, element: null }
  ).element;
}

function reorderFromDom() {
  columnBodies.forEach((body) => {
    const status = body.parentElement.dataset.status;
    [...body.querySelectorAll('.card')].forEach((card, index) => {
      const task = tasks.find((item) => item.id === card.dataset.id);
      if (task) {
        task.status = status;
        task.order = index + 1;
      }
    });
  });
}

function updateCounts(filtered) {
  const countsByStatus = filtered.reduce((acc, task) => {
    acc[task.status] = (acc[task.status] || 0) + 1;
    return acc;
  }, {});

  Object.entries(counts).forEach(([status, el]) => {
    el.textContent = countsByStatus[status] || 0;
  });
}

function updateStats(filtered) {
  stats.total.textContent = filtered.length;
  stats.progress.textContent = filtered.filter((t) => t.status === 'in-progress').length;
  stats.done.textContent = filtered.filter((t) => t.status === 'done').length;
  const deadlines = filtered
    .filter((t) => t.dueDate && t.status !== 'done')
    .sort((a, b) => new Date(a.dueDate) - new Date(b.dueDate));
  stats.deadline.textContent = deadlines.length
    ? new Date(deadlines[0].dueDate).toLocaleDateString()
    : 'None';
}

function openModal(task = null) {
  form.reset();
  form.querySelector('#task-id').value = task?.id || '';
  form.querySelector('#title').value = task?.title || '';
  form.querySelector('#description').value = task?.description || '';
  form.querySelector('#assignee').value = task?.assignee || '';
  form.querySelector('#priority').value = task?.priority || 'medium';
  form.querySelector('#status').value = task?.status || 'backlog';
  form.querySelector('#dueDate').value = task?.dueDate || '';
  document.getElementById('modal-title').textContent = task ? 'Edit Task' : 'New Task';
  deleteBtn.style.visibility = task ? 'visible' : 'hidden';
  modal.showModal();
}

function loadTasks() {
  try {
    const raw = localStorage.getItem('kanban-tasks');
    return raw ? JSON.parse(raw) : [];
  } catch {
    return [];
  }
}

function saveTasks() {
  localStorage.setItem('kanban-tasks', JSON.stringify(tasks));
}

function getNextOrder(status) {
  const items = tasks.filter((task) => task.status === status);
  return items.length ? Math.max(...items.map((t) => t.order || 0)) + 1 : 1;
}

function isOverdue(task) {
  if (!task.dueDate) return false;
  const today = new Date();
  const due = new Date(task.dueDate);
  return due < today && task.status !== 'done';
}

function createSeedData() {
  const today = new Date();
  const addDays = (days) => new Date(today.getTime() + days * 86400000).toISOString().slice(0, 10);
  return [
    {
      id: crypto.randomUUID(),
      title: 'Design landing page hero',
      description: 'Create final mockups for the hero section with CTA and metrics.',
      assignee: 'Alex',
      priority: 'high',
      status: 'backlog',
      dueDate: addDays(5),
      createdAt: today.toISOString(),
      updatedAt: today.toISOString(),
      order: 1
    },
    {
      id: crypto.randomUUID(),
      title: 'API contract review',
      description: 'Review API spec with backend and annotate field requirements.',
      assignee: 'Sam',
      priority: 'medium',
      status: 'todo',
      dueDate: addDays(2),
      createdAt: today.toISOString(),
      updatedAt: today.toISOString(),
      order: 1
    },
    {
      id: crypto.randomUUID(),
      title: 'Implement auth guard',
      description: 'Protect dashboard routes and refresh tokens automatically.',
      assignee: 'Priya',
      priority: 'high',
      status: 'in-progress',
      dueDate: addDays(1),
      createdAt: today.toISOString(),
      updatedAt: today.toISOString(),
      order: 1
    },
    {
      id: crypto.randomUUID(),
      title: 'QA checklist',
      description: 'Draft regression checklist for release candidate.',
      assignee: 'Diego',
      priority: 'low',
      status: 'review',
      dueDate: addDays(3),
      createdAt: today.toISOString(),
      updatedAt: today.toISOString(),
      order: 1
    },
    {
      id: crypto.randomUUID(),
      title: 'Deploy pipeline clean-up',
      description: 'Remove deprecated steps and document rollback procedure.',
      assignee: 'Taylor',
      priority: 'medium',
      status: 'done',
      dueDate: addDays(-1),
      createdAt: today.toISOString(),
      updatedAt: today.toISOString(),
      order: 1
    }
  ];
}
