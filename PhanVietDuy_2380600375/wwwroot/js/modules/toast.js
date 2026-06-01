/**
 * toast.js — Toast notification system
 * Usage: showToast('Đã thêm vào giỏ hàng ✓', 'success')
 *        showToast('Có lỗi xảy ra. Vui lòng thử lại.', 'error')
 *        showToast('Thông tin', 'info')
 */

export function showToast(message, type = 'info', duration = 4000) {
  const container = document.getElementById('toastContainer');
  if (!container) return;

  const toast = document.createElement('div');
  toast.className = `toast toast--${type}`;
  toast.setAttribute('role', 'status');
  toast.innerHTML = `
    <span>${message}</span>
    <button class="toast__close" aria-label="Đóng thông báo" type="button">
      <i class="fa-solid fa-xmark" aria-hidden="true"></i>
    </button>`;

  container.appendChild(toast);

  const remove = () => {
    toast.style.animation = 'slideOutRight 250ms var(--ease-out) forwards';
    toast.addEventListener('animationend', () => toast.remove(), { once: true });
  };

  toast.querySelector('.toast__close')?.addEventListener('click', remove);

  setTimeout(remove, duration);

  return toast;
}

/**
 * Init toasts from server-side TempData.
 * The Layout passes toast data in data-toast attribute on <body>.
 */
export function initToasts() {
  const raw = document.body.dataset.toast;
  if (!raw || raw === '""') return;
  try {
    const toast = JSON.parse(raw);
    if (toast?.message) showToast(toast.message, toast.type ?? 'info');
  } catch { /* ignore */ }
}
