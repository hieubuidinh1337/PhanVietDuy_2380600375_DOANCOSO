/**
 * gallery.js — Product image gallery module
 * Handles: thumbnail switching, prev/next navigation, keyboard support
 */
export function initGallery() {
  const galleryMain = document.getElementById('galleryMain');
  const mainImg     = document.getElementById('mainImage');
  const thumbs      = document.querySelectorAll('.gallery-thumb');
  const prevBtn     = document.getElementById('galleryPrev');
  const nextBtn     = document.getElementById('galleryNext');
  const countEl     = document.getElementById('galleryCount');

  if (!galleryMain || !mainImg) return;

  let current = 0;
  const images = Array.from(thumbs).map(t => t.dataset.src ?? '');
  const total  = images.length;

  function goTo(index) {
    if (total === 0) return;
    current = (index + total) % total;

    // Fade transition
    mainImg.style.opacity = '0';
    mainImg.style.transition = 'opacity 200ms';
    requestAnimationFrame(() => {
      mainImg.src = images[current];
      mainImg.onload = () => {
        mainImg.style.opacity = '1';
      };
    });

    // Update thumbs
    thumbs.forEach((t, i) => {
      t.classList.toggle('is-active', i === current);
      t.setAttribute('aria-pressed', String(i === current));
    });

    // Update count
    if (countEl) countEl.textContent = `${current + 1} / ${total}`;
  }

  // Thumb click
  thumbs.forEach((thumb, i) => {
    thumb.addEventListener('click', () => goTo(i));
  });

  // Arrow click
  prevBtn?.addEventListener('click', () => goTo(current - 1));
  nextBtn?.addEventListener('click', () => goTo(current + 1));

  // Keyboard
  galleryMain.setAttribute('tabindex', '0');
  galleryMain.addEventListener('keydown', e => {
    if (e.key === 'ArrowRight') goTo(current + 1);
    if (e.key === 'ArrowLeft')  goTo(current - 1);
  });

  // Touch swipe
  let startX = 0;
  galleryMain.addEventListener('touchstart', e => {
    startX = e.changedTouches[0].clientX;
  }, { passive: true });
  galleryMain.addEventListener('touchend', e => {
    const delta = e.changedTouches[0].clientX - startX;
    if (Math.abs(delta) > 50) {
      goTo(delta < 0 ? current + 1 : current - 1);
    }
  });
}

/**
 * Accordion component
 */
export function initAccordion() {
  document.querySelectorAll('.accordion__trigger').forEach(trigger => {
    trigger.addEventListener('click', () => {
      const expanded = trigger.getAttribute('aria-expanded') === 'true';
      const content  = trigger.nextElementSibling;

      // Close all in same accordion
      const accordion = trigger.closest('.accordion');
      accordion?.querySelectorAll('.accordion__trigger').forEach(t => {
        t.setAttribute('aria-expanded', 'false');
        const c = t.nextElementSibling;
        c?.classList.remove('is-open');
      });

      // Toggle current
      if (!expanded) {
        trigger.setAttribute('aria-expanded', 'true');
        content?.classList.add('is-open');
      }
    });
  });
}

/**
 * Quantity stepper (product detail)
 */
export function initQtyStepper() {
  const qtyEl = document.getElementById('qtyValue');
  if (!qtyEl) return;

  const maxStock = parseInt(document.getElementById('addToCartBtn')?.dataset.stock ?? '99');
  let qty = 1;

  document.querySelectorAll('.qty-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      if (btn.dataset.action === 'increase') {
        qty = Math.min(qty + 1, maxStock);
      } else {
        qty = Math.max(qty - 1, 1);
      }
      qtyEl.textContent = String(qty);
    });
  });

  return () => qty; // expose getter
}

/**
 * Color / Size selectors (product detail)
 */
export function initSelectors() {
  // Color
  const colorLabel = document.getElementById('colorLabel');
  document.querySelectorAll('.color-swatch').forEach(swatch => {
    swatch.addEventListener('click', () => {
      document.querySelectorAll('.color-swatch').forEach(s => {
        s.classList.remove('is-active');
        s.setAttribute('aria-pressed', 'false');
      });
      swatch.classList.add('is-active');
      swatch.setAttribute('aria-pressed', 'true');
      if (colorLabel) colorLabel.textContent = swatch.dataset.color ?? '';
    });
  });

  // Size
  const sizeLabel = document.getElementById('sizeLabel');
  document.querySelectorAll('.size-btn:not(:disabled)').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.size-btn').forEach(b => b.classList.remove('is-active'));
      btn.classList.add('is-active');
      if (sizeLabel) sizeLabel.textContent = btn.dataset.size ?? '';
    });
  });
}
