/**
 * cart.js — Cart drawer module
 * Handles: open/close, fetch items, add/remove/update, coupon, totals
 */

const CART_API = '/cart';
const currency = (n) => `${Number(n).toLocaleString('vi-VN')} ₫`;

let state = {
  items: [],
  subtotal: 0,
  discount: 0,
  total: 0,
  itemCount: 0,
};

// ── Elements ──
const drawer  = () => document.getElementById('cartDrawer');
const scrim   = () => document.getElementById('cartScrim');
const body    = () => document.getElementById('cartBody');
const footer  = () => document.getElementById('cartFooter');
const emptyEl = () => document.getElementById('cartEmpty');
const countEl = () => document.getElementById('cartCount');
const drawerCount = () => document.getElementById('cartDrawerCount');

function getCsrfToken() {
  return document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') ?? '';
}

// ── Open / Close ──
function openCart() {
  drawer()?.classList.add('is-open');
  scrim()?.classList.add('is-visible');
  drawer()?.setAttribute('aria-hidden', 'false');
  document.getElementById('cartToggle')?.setAttribute('aria-expanded', 'true');
  document.body.style.overflow = 'hidden';
  fetchCartItems();
}

function closeCart() {
  drawer()?.classList.remove('is-open');
  scrim()?.classList.remove('is-visible');
  drawer()?.setAttribute('aria-hidden', 'true');
  document.getElementById('cartToggle')?.setAttribute('aria-expanded', 'false');
  document.body.style.overflow = '';
}

// ── Fetch cart from server ──
async function fetchCartItems() {
  try {
    const res = await fetch(`${CART_API}/items`, { credentials: 'same-origin' });
    if (!res.ok) throw new Error('Fetch failed');
    const data = await res.json();
    state = data;
    renderCart();
  } catch {
    console.error('[Cart] Failed to load cart.');
  }
}

// ── Render ──
function renderCart() {
  const bodyEl  = body();
  const footerEl = footer();
  const emptyDiv = emptyEl();

  if (!bodyEl) return;

  if (!state.items?.length) {
    if (emptyDiv) emptyDiv.style.display = '';
    if (footerEl) footerEl.style.display = 'none';
    updateBadge(0);
    return;
  }

  if (emptyDiv) emptyDiv.style.display = 'none';
  if (footerEl) footerEl.style.display = '';

  // Remove old items (keep empty div)
  bodyEl.querySelectorAll('.cart-item').forEach(el => el.remove());

  // Render items
  const frag = document.createDocumentFragment();
  state.items.forEach(item => {
    const el = buildCartItem(item);
    frag.appendChild(el);
  });

  // Insert before empty div
  if (emptyDiv) {
    bodyEl.insertBefore(frag, emptyDiv);
  } else {
    bodyEl.appendChild(frag);
  }

  // Update totals
  const subtotalEl = document.getElementById('cartSubtotal');
  const discountRow = document.getElementById('discountRow');
  const discountEl = document.getElementById('cartDiscount');
  const totalEl    = document.getElementById('cartTotal');

  if (subtotalEl) subtotalEl.textContent = currency(state.subtotal);
  if (discountRow) discountRow.style.display = state.discount > 0 ? '' : 'none';
  if (discountEl)  discountEl.textContent = `-${currency(state.discount)}`;
  if (totalEl)     totalEl.textContent = currency(state.total);

  updateBadge(state.itemCount);
}

// ── Build item element ──
function buildCartItem(item) {
  const div = document.createElement('div');
  div.className = 'cart-item';
  div.dataset.id = item.id;
  div.innerHTML = `
    <div class="cart-item__img">
      <img src="${item.thumbnailUrl ?? '/images/placeholder-product.webp'}" alt="${item.productName}" width="64" height="64" loading="lazy" />
    </div>
    <div class="cart-item__info">
      <div class="cart-item__name">${item.productName}</div>
      ${item.selectedVariant ? `<div class="cart-item__variant">${item.selectedVariant}</div>` : ''}
      <div class="cart-item__row">
        <div class="cart-item__qty" role="group" aria-label="Số lượng">
          <button class="cart-item__qty-btn" data-action="decrease" aria-label="Giảm" type="button">−</button>
          <span class="cart-item__qty-val" aria-live="polite">${item.quantity}</span>
          <button class="cart-item__qty-btn" data-action="increase" aria-label="Tăng" type="button">+</button>
        </div>
        <span class="cart-item__price">${currency(item.lineTotal)}</span>
      </div>
    </div>
    <button class="cart-item__remove" aria-label="Xóa ${item.productName}" data-id="${item.id}" type="button">
      <i class="fa-solid fa-xmark" aria-hidden="true"></i>
    </button>`;

  // Qty buttons
  div.querySelectorAll('.cart-item__qty-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      const action = btn.dataset.action;
      const newQty = action === 'increase' ? item.quantity + 1 : item.quantity - 1;
      if (newQty < 1) { removeItem(item.id, div); return; }
      updateQty(item.id, newQty);
    });
  });

  // Remove button
  div.querySelector('.cart-item__remove')?.addEventListener('click', () => removeItem(item.id, div));

  return div;
}

// ── Add to cart ──
export async function addToCart(productId, qty = 1, variant = '') {
  try {
    const res = await fetch(`${CART_API}/add`, {
      method: 'POST',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json',
        'RequestVerificationToken': getCsrfToken(),
      },
      body: JSON.stringify({ productId, quantity: qty, selectedVariant: variant }),
    });

    if (!res.ok) throw new Error('Add failed');
    const data = await res.json();
    state = data;

    // Bump badge animation
    const countBadge = document.getElementById('cartCount');
    countBadge?.classList.add('bump');
    setTimeout(() => countBadge?.classList.remove('bump'), 300);

    updateBadge(data.itemCount);
    return data;
  } catch (e) {
    console.error('[Cart] Add failed', e);
    throw e;
  }
}

// ── Update qty ──
async function updateQty(cartItemId, qty) {
  try {
    const res = await fetch(`${CART_API}/update`, {
      method: 'POST',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json',
        'RequestVerificationToken': getCsrfToken(),
      },
      body: JSON.stringify({ cartItemId, quantity: qty }),
    });
    if (!res.ok) throw new Error('Update failed');
    const data = await res.json();
    state = data;
    renderCart();
  } catch { /* ignore */ }
}

// ── Remove item ──
async function removeItem(cartItemId, el) {
  el.classList.add('is-removing');
  await new Promise(r => setTimeout(r, 280));
  try {
    await fetch(`${CART_API}/remove/${cartItemId}`, {
      method: 'DELETE',
      credentials: 'same-origin',
      headers: { 'RequestVerificationToken': getCsrfToken() },
    });
    await fetchCartItems();
  } catch { await fetchCartItems(); }
}

// ── Update badge ──
function updateBadge(count) {
  [countEl(), drawerCount()].forEach(el => {
    if (!el) return;
    if (count > 0) {
      el.textContent = count > 99 ? '99+' : String(count);
      el.removeAttribute('hidden');
    } else {
      el.textContent = '';
    }
  });
}

// ── Apply coupon ──
async function applyCoupon(code, msgEl) {
  try {
    const res = await fetch(`${CART_API}/coupon`, {
      method: 'POST',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json',
        'RequestVerificationToken': getCsrfToken(),
      },
      body: JSON.stringify({ code }),
    });
    const data = await res.json();
    if (msgEl) {
      msgEl.textContent = data.message;
      msgEl.style.color = data.success ? 'var(--c-success)' : 'var(--c-danger)';
    }
    if (data.success) {
      state = data.cart;
      renderCart();
    }
  } catch { /* ignore */ }
}

// ── Init ──
export function initCart() {
  // Open/close
  document.getElementById('cartToggle')?.addEventListener('click', openCart);
  document.getElementById('cartClose')?.addEventListener('click', closeCart);
  document.getElementById('cartClose2')?.addEventListener('click', closeCart);
  document.getElementById('cartClose2b')?.addEventListener('click', closeCart);
  scrim()?.addEventListener('click', closeCart);

  // Keyboard close
  document.addEventListener('keydown', e => {
    if (e.key === 'Escape' && drawer()?.classList.contains('is-open')) closeCart();
  });

  // Coupon
  document.getElementById('couponApply')?.addEventListener('click', () => {
    const input = document.getElementById('couponInput');
    const code = input ? input.value.trim() : '';
    if (code) applyCoupon(code, document.getElementById('couponMsg'));
  });

  // Fetch initial count on page load
  fetch(`${CART_API}/count`, { credentials: 'same-origin' })
    .then(r => r.json())
    .then(d => updateBadge(d.count ?? 0))
    .catch(() => {});
}
