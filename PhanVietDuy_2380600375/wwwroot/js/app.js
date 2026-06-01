/**
 * app.js — Application entry point (ES Module)
 * Imports and orchestrates all JS modules.
 */
import { initNav }    from './modules/nav.js';
import { initCart, addToCart } from './modules/cart.js';
import { initToasts, showToast } from './modules/toast.js';
import { initGallery, initAccordion, initQtyStepper, initSelectors } from './modules/gallery.js';
import { initReveal } from './modules/reveal.js';
import { initCursor } from './modules/cursor.js';

// ── Make showToast globally accessible for inline scripts ──
window.showToast = showToast;
window.addToCart = addToCart;

document.addEventListener('DOMContentLoaded', () => {
  // ── Core modules (every page) ──
  initCursor();
  initNav();
  initCart();
  initToasts();
  initReveal();

  const page = document.body.dataset.page;

  // ── Product detail page ──
  if (page === 'product-detail') {
    initGallery();
    initAccordion();
    initSelectors();
    const getQty = initQtyStepper();

    // Add to cart integration
    const addBtn = document.getElementById('addToCartBtn');
    addBtn?.addEventListener('click', async () => {
      const productId = addBtn.dataset.productId;
      const qty = getQty?.() ?? 1;
      const color = document.getElementById('colorLabel')?.textContent?.trim();
      const size  = document.getElementById('sizeLabel')?.textContent?.trim();
      const variant = [color !== 'Chọn màu' ? color : '', size !== 'Chọn size' ? size : '']
        .filter(Boolean).join(' / ');

      addBtn.disabled = true;
      addBtn.innerHTML = '<span class="spinner"></span>';

      try {
        await addToCart(productId, qty, variant);
        addBtn.textContent = '✓ Đã thêm';
        addBtn.classList.add('is-added');
        showToast('Đã thêm vào giỏ hàng ✓', 'success');

        setTimeout(() => {
          addBtn.disabled = false;
          addBtn.textContent = 'Thêm vào giỏ hàng';
          addBtn.classList.remove('is-added');
        }, 2000);

        // Open cart drawer
        const cartToggle = document.getElementById('cartToggle');
        setTimeout(() => cartToggle?.click(), 500);

      } catch {
        addBtn.disabled = false;
        addBtn.textContent = 'Thêm vào giỏ hàng';
        showToast('Có lỗi xảy ra. Vui lòng thử lại.', 'error');
      }
    });

    // Wishlist button
    const wishBtn = document.getElementById('wishlistBtn');
    wishBtn?.addEventListener('click', async () => {
      const productId = wishBtn.dataset.productId;
      const isActive  = wishBtn.getAttribute('aria-pressed') === 'true';
      try {
        const res = await fetch(`/wishlist/toggle`, {
          method: 'POST',
          credentials: 'same-origin',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') ?? '',
          },
          body: JSON.stringify({ productId }),
        });
        const data = await res.json();
        wishBtn.setAttribute('aria-pressed', String(data.isWishlisted));
        wishBtn.classList.toggle('is-active', data.isWishlisted);
        const icon = wishBtn.querySelector('i');
        if (icon) icon.className = data.isWishlisted ? 'fa-solid fa-heart' : 'fa-regular fa-heart';
        showToast(data.isWishlisted ? 'Đã thêm vào yêu thích ♡' : 'Đã xóa khỏi yêu thích', 'info');
      } catch { /* ignore */ }
    });
  }

  // ── Product card quick-view + wishlist ──
  const currencyFormatter = (n) => `${Number(n).toLocaleString('vi-VN')} ₫`;

  function getColorHexFromName(colorName) {
    const map = {
      'Silver': '#E1E1E1',
      'Platinum': '#E5E4E2',
      'Rose Gold': '#B76E79',
      'White Gold': '#F4F0EA',
      'Gold': '#D4AF37',
      'Diamond': '#E5F4F4',
      'Black': '#1A1A1A',
      'Brown': '#70543E',
      'Navy': '#002040',
      'Red': '#A92424',
      'Blue': '#3B5998',
      'Green': '#2E5A44',
      'Charcoal': '#36454F',
      'Yellow': '#FFDE59',
      'White': '#FFFFFF',
      'Platinum / Brown': '#70543E',
      'Platinum / Black': '#1A1A1A',
      'Platinum / Blue': '#002040',
      'Rose Gold / Black': '#1A1A1A',
      'Rose Gold / Brown': '#70543E',
      'Rose Gold / Navy': '#002040',
      'Navy Blue': '#002040',
      'Forest Green': '#224D33',
      'Bordeaux Red': '#5C0612',
      'Warm White': '#FAF9F6',
      'Space Grey': '#34383D',
      'Onyx Black': '#0A0A0A',
      'Midnight Blue': '#191970',
      'Sand Beige': '#E5D3B3'
    };
    return map[colorName] || '#E4DED6';
  }

  async function openQuickView(productId) {
    let backdrop = document.getElementById('luxuryQuickViewBackdrop');
    if (!backdrop) {
      backdrop = document.createElement('div');
      backdrop.id = 'luxuryQuickViewBackdrop';
      backdrop.className = 'luxury-quickview-backdrop';
      document.body.appendChild(backdrop);
    }
    
    backdrop.innerHTML = `<div class="luxury-quickview-spinner"></div>`;
    backdrop.classList.add('is-active');
    document.body.style.overflow = 'hidden';

    const dismissBackdrop = (e) => {
      if (e.target === backdrop) {
        closeQuickView();
      }
    };
    backdrop.addEventListener('click', dismissBackdrop);

    function closeQuickView() {
      backdrop.classList.remove('is-active');
      document.body.style.overflow = '';
      backdrop.removeEventListener('click', dismissBackdrop);
    }

    try {
      const response = await fetch(`/product/quick-view/${productId}`);
      if (!response.ok) throw new Error();
      const product = await response.json();

      const hasDiscount = product.originalPrice && product.originalPrice > product.price;
      const discountPct = hasDiscount ? Math.round((1 - product.price / product.originalPrice) * 100) : 0;
      
      const badgeHtml = product.badge ? `
        <span class="badge ${product.badgeStyle === 'exclusive' ? 'badge--exclusive' : product.badgeStyle === 'new' ? 'badge--new' : 'badge--secondary'}">
          ${product.badge}
        </span>
      ` : '';

      const thumbsHtml = product.images && product.images.length > 1 ? `
        <div class="gallery-thumbs">
          ${product.images.map((img, idx) => `
            <div class="gallery-thumb ${idx === 0 ? 'is-active' : ''}" data-img="${img}">
              <img src="${img}" alt="${product.name} Thumbnail" />
            </div>
          `).join('')}
        </div>
      ` : '';

      const colorOptionHtml = product.colors && product.colors.length > 0 ? `
        <div class="option-group">
          <span class="option-label">Màu sắc: <strong class="selected-option-label" id="qvColorVal">${product.colors[0]}</strong></span>
          <div class="color-swatches">
            ${product.colors.map((color, idx) => `
              <button class="color-swatch ${idx === 0 ? 'is-active' : ''}" 
                      data-color="${color}" 
                      style="background-color: ${getColorHexFromName(color)}" 
                      title="${color}"
                      type="button"></button>
            `).join('')}
          </div>
        </div>
      ` : '';

      const sizeOptionHtml = product.sizes && product.sizes.length > 0 ? `
        <div class="option-group">
          <span class="option-label">Size: <strong class="selected-option-label" id="qvSizeVal">${product.sizes[0]}</strong></span>
          <div class="size-buttons">
            ${product.sizes.map((size, idx) => `
              <button class="size-btn ${idx === 0 ? 'is-active' : ''}" 
                      data-size="${size}" 
                      type="button">${size}</button>
            `).join('')}
          </div>
        </div>
      ` : '';

      backdrop.innerHTML = `
        <div class="luxury-quickview-modal">
          <button class="luxury-quickview-close" id="qvCloseBtn" aria-label="Đóng">&times;</button>
          <div class="luxury-quickview-body">
            <div class="luxury-quickview-gallery">
              <div class="gallery-main-container">
                <img class="gallery-main-img" id="qvMainImg" src="${product.thumbnailUrl || ''}" alt="${product.name}" />
              </div>
              ${thumbsHtml}
            </div>
            
            <div class="luxury-quickview-info">
              <div class="info-header">
                ${badgeHtml}
                <h2 class="product-title">${product.name}</h2>
                <div class="product-price-row">
                  <span class="price">${currencyFormatter(product.price)}</span>
                  ${hasDiscount ? `
                    <span class="original-price">${currencyFormatter(product.originalPrice)}</span>
                    <span class="discount-badge">-${discountPct}%</span>
                  ` : ''}
                </div>
              </div>
              
              <p class="product-desc">${product.shortDesc || 'Sản phẩm thủ công tinh xảo độc quyền của Atelier Vietduy.'}</p>
              <div class="divider"></div>
              
              ${colorOptionHtml}
              ${sizeOptionHtml}
              
              <div class="purchase-row">
                <div class="qty-stepper">
                  <button class="qty-btn" id="qvQtyDec" type="button">−</button>
                  <span class="qty-value" id="qvQtyVal">1</span>
                  <button class="qty-btn" id="qvQtyInc" type="button">+</button>
                </div>
                
                <button class="btn btn--solid btn--lg qv-add-to-cart-btn" 
                        id="qvAddToCartBtn" 
                        data-product-id="${product.id}" 
                        ${!product.isActive ? 'disabled' : ''} 
                        type="button">
                  ${product.isActive ? 'THÊM VÀO GIỎ HÀNG' : 'TẠM HẾT HÀNG'}
                </button>
              </div>
              
              <a href="/product/${product.slug}" class="view-full-details-link">
                Xem chi tiết đầy đủ sản phẩm <i class="fa-solid fa-arrow-right-long"></i>
              </a>
            </div>
          </div>
        </div>
      `;

      document.getElementById('qvCloseBtn').addEventListener('click', closeQuickView);

      backdrop.querySelectorAll('.gallery-thumb').forEach(thumb => {
        thumb.addEventListener('click', () => {
          backdrop.querySelectorAll('.gallery-thumb').forEach(t => t.classList.remove('is-active'));
          thumb.classList.add('is-active');
          document.getElementById('qvMainImg').src = thumb.dataset.img;
        });
      });

      let selectedColor = product.colors && product.colors.length > 0 ? product.colors[0] : null;
      backdrop.querySelectorAll('.color-swatch').forEach(swatch => {
        swatch.addEventListener('click', () => {
          backdrop.querySelectorAll('.color-swatch').forEach(s => s.classList.remove('is-active'));
          swatch.classList.add('is-active');
          selectedColor = swatch.dataset.color;
          const label = document.getElementById('qvColorVal');
          if (label) label.textContent = selectedColor;
        });
      });

      let selectedSize = product.sizes && product.sizes.length > 0 ? product.sizes[0] : null;
      backdrop.querySelectorAll('.size-btn').forEach(btn => {
        btn.addEventListener('click', () => {
          backdrop.querySelectorAll('.size-btn').forEach(b => b.classList.remove('is-active'));
          btn.classList.add('is-active');
          selectedSize = btn.dataset.size;
          const label = document.getElementById('qvSizeVal');
          if (label) label.textContent = selectedSize;
        });
      });

      let quantity = 1;
      const qtyVal = document.getElementById('qvQtyVal');
      document.getElementById('qvQtyDec').addEventListener('click', () => {
        if (quantity > 1) {
          quantity--;
          qtyVal.textContent = quantity;
        }
      });
      document.getElementById('qvQtyInc').addEventListener('click', () => {
        quantity++;
        qtyVal.textContent = quantity;
      });

      const addBtn = document.getElementById('qvAddToCartBtn');
      addBtn.addEventListener('click', async () => {
        addBtn.disabled = true;
        addBtn.textContent = 'ĐANG XỬ LÝ...';
        try {
          let variant = '';
          if (selectedColor && selectedSize) {
            variant = `${selectedColor} / ${selectedSize}`;
          } else if (selectedColor) {
            variant = selectedColor;
          }
          
          await addToCart(product.id, quantity, variant);
          showToast('Đã thêm vào giỏ hàng ✓', 'success');
          closeQuickView();
          
          const cartToggle = document.getElementById('cartToggle');
          setTimeout(() => cartToggle?.click(), 300);
        } catch (err) {
          showToast(err.message || 'Có lỗi xảy ra. Vui lòng thử lại.', 'error');
          addBtn.disabled = false;
          addBtn.textContent = 'THÊM VÀO GIỎ HÀNG';
        }
      });

    } catch (err) {
      showToast('Không thể tải thông tin sản phẩm. Vui lòng thử lại.', 'error');
      closeQuickView();
    }
  }

  document.querySelectorAll('.product-card__quick').forEach(btn => {
    btn.addEventListener('click', (e) => {
      e.preventDefault();
      e.stopPropagation();
      openQuickView(btn.dataset.productId);
    });
  });

  document.querySelectorAll('.product-card__wish').forEach(btn => {
    btn.addEventListener('click', async (e) => {
      e.preventDefault();
      e.stopPropagation();
      const productId = btn.dataset.productId;
      btn.classList.toggle('is-active');
      const icon = btn.querySelector('i');
      if (icon) icon.className = btn.classList.contains('is-active') ? 'fa-solid fa-heart' : 'fa-regular fa-heart';
      
      try {
        const res = await fetch(`/wishlist/toggle`, {
          method: 'POST',
          credentials: 'same-origin',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') ?? '',
          },
          body: JSON.stringify({ productId }),
        });
        const data = await res.json();
        showToast(data.isWishlisted ? 'Đã thêm vào yêu thích ♡' : 'Đã xóa khỏi yêu thích', 'info');
      } catch { /* ignore */ }
    });
  });

  // ── Newsletter forms ──
  document.querySelectorAll('form[action*="newsletter"]').forEach(form => {
    form.addEventListener('submit', async (e) => {
      e.preventDefault();
      const email = form.querySelector('input[type=email]')?.value;
      if (!email) return;
      try {
        const token = document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') ?? '';
        await fetch('/newsletter/subscribe', {
          method: 'POST',
          credentials: 'same-origin',
          headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
          body: JSON.stringify({ email }),
        });
        form.reset();
        showToast('Đăng ký thành công! Cảm ơn bạn.', 'success');
      } catch { showToast('Có lỗi xảy ra. Vui lòng thử lại.', 'error'); }
    });
  });

  // ── Admin accordion / data table search ──
  if (document.querySelector('.admin-layout')) {
    // Chart bar interaction (if present)
    document.querySelectorAll('.chart-bar').forEach(bar => {
      bar.setAttribute('tabindex', '0');
      bar.addEventListener('keydown', e => {
        if (e.key === 'Enter' || e.key === ' ') {
          bar.click();
        }
      });
    });
  }
});
