/**
 * nav.js — Navigation module
 * Handles: header scroll state, mobile toggle, user dropdown, search overlay
 */
export function initNav() {
  const header     = document.getElementById('siteHeader');
  const navToggle  = document.getElementById('navToggle');
  const navLinks   = document.getElementById('navLinks');
  const searchToggle = document.getElementById('searchToggle');
  const searchClose  = document.getElementById('searchClose');
  const searchOverlay = document.getElementById('searchOverlay');
  const searchInput   = document.getElementById('searchInput');
  const userToggle    = document.getElementById('userToggle');
  const userMenu      = document.getElementById('userMenu');

  // ── Header scroll elevation ──
  const onScroll = () => {
    header?.classList.toggle('is-scrolled', window.scrollY > 20);
  };
  window.addEventListener('scroll', onScroll, { passive: true });
  onScroll(); // run immediately

  // ── Mobile nav toggle ──
  navToggle?.addEventListener('click', () => {
    const isOpen = navLinks?.classList.toggle('is-open');
    navToggle.classList.toggle('is-active', isOpen);
    navToggle.setAttribute('aria-expanded', String(isOpen));
    document.body.style.overflow = isOpen ? 'hidden' : '';
  });

  // Close nav when link clicked (mobile)
  navLinks?.querySelectorAll('.nav__link').forEach(link => {
    link.addEventListener('click', () => {
      navLinks.classList.remove('is-open');
      navToggle?.classList.remove('is-active');
      navToggle?.setAttribute('aria-expanded', 'false');
      document.body.style.overflow = '';
    });
  });

  // ── Search overlay ──
  const openSearch = () => {
    searchOverlay?.removeAttribute('hidden');
    searchToggle?.setAttribute('aria-expanded', 'true');
    searchInput?.focus();
  };
  const closeSearch = () => {
    searchOverlay?.setAttribute('hidden', '');
    searchToggle?.setAttribute('aria-expanded', 'false');
    searchToggle?.focus();
  };

  searchToggle?.addEventListener('click', openSearch);
  searchClose?.addEventListener('click', closeSearch);
  searchOverlay?.addEventListener('keydown', e => {
    if (e.key === 'Escape') closeSearch();
  });

  // ── User dropdown ──
  userToggle?.addEventListener('click', () => {
    const isOpen = userMenu?.classList.toggle('is-open');
    userToggle.setAttribute('aria-expanded', String(isOpen));
    userMenu?.setAttribute('aria-hidden', String(!isOpen));
  });

  document.addEventListener('click', e => {
    const dropdown = document.getElementById('userDropdown');
    if (!dropdown?.contains(e.target)) {
      userMenu?.classList.remove('is-open');
      userToggle?.setAttribute('aria-expanded', 'false');
      userMenu?.setAttribute('aria-hidden', 'true');
    }
  });

  document.addEventListener('keydown', e => {
    if (e.key === 'Escape') {
      closeSearch();
      userMenu?.classList.remove('is-open');
      userToggle?.setAttribute('aria-expanded', 'false');
    }
  });
}

/** Bump cart count badge with spring animation */
export function bumpCartCount(newCount) {
  const badge = document.getElementById('cartCount');
  if (!badge) return;
  badge.textContent = newCount > 0 ? newCount : '';
  badge.classList.remove('bump');
  void badge.offsetWidth; // force reflow
  badge.classList.add('bump');
  setTimeout(() => badge.classList.remove('bump'), 300);
}
