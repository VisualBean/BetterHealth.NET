let refreshInterval = parseInt(localStorage.getItem('refreshInterval')) || 30;
let intervalId = null;

function startAutoRefresh() {
  if (intervalId) clearInterval(intervalId);
  intervalId = setInterval(loadHealthData, refreshInterval * 1000);
  document.getElementById('refresh-label').textContent = `Refresh: ${refreshInterval}s`;
}

function setupDropdown() {
  const dropdown = document.getElementById('refresh-dropdown');
  const trigger = dropdown.querySelector('.dropdown-trigger button');
  const items = dropdown.querySelectorAll('.dropdown-item');

  trigger.addEventListener('click', () => {
    dropdown.classList.toggle('is-active');
  });

  items.forEach(item => {
    item.addEventListener('click', (e) => {
      e.preventDefault();
      refreshInterval = parseInt(item.getAttribute('data-interval'));
      localStorage.setItem('refreshInterval', refreshInterval);
      dropdown.classList.remove('is-active');
      startAutoRefresh();
    });
  });

  document.addEventListener('click', (e) => {
    if (!dropdown.contains(e.target)) {
      dropdown.classList.remove('is-active');
    }
  });
}

async function loadHealthData() {
  try {
    const res = await fetch('data');
    if (!res.ok) throw new Error('Failed to load health data');

    const data = await res.json();

    const overall = document.getElementById('overall-status');
    overall.textContent = data.status;
    overall.className = `status ${data.status}`;

    const container = document.getElementById('health-checks-container');
    container.innerHTML = '';

    data.checks.forEach(check => {
      const card = createHealthCheckCard(check);
      container.appendChild(card);
    });
  } catch (err) {
    console.error(err);
    document.getElementById('overall-status').textContent = 'Error';
    document.getElementById('overall-status').className = 'status Unhealthy';
  }
}

function createHealthCheckCard(check) {
    const card = document.createElement('div');
    card.innerHTML = `
    <div class="status-card status-card--hoverable">
        <div class="status-card__status-bar status-card__status--${check.status}"></div>

        <div class="status-card__title">${check.name}</div>
        <div class="status-card__status-label status-card__status-text--${check.status}">${check.status}</div>
        <div class="status-card__description">
            ${check.description || 'No description'} <br>
            Duration: ${check.duration}
        </div>

        <div class="status-card__tags tags" id="tags-container">
    
        </div>
    </div>
    `;

  const tagsContainer = card.querySelector('#tags-container');
  if (check.tags && check.tags.length > 0) {
    check.tags.forEach(tag => {
      const tagElement = document.createElement('span');
      tagElement.classList.add('tag');
      tagElement.textContent = tag;
      tagsContainer.appendChild(tagElement);
    });
  }

  return card;
}

setupDropdown();
loadHealthData();
startAutoRefresh();