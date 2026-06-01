/**
 * reveal.js — Intersection Observer-based reveal animations v2.0
 * Supports: .reveal, .reveal-fade, .reveal-clip, .reveal-rule
 * Also handles: product card stagger via --card-delay CSS var
 */
export function initReveal() {
  const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  // All reveal classes
  const allRevealEls = document.querySelectorAll(
    '.reveal, .reveal-fade, .reveal-clip, .reveal-rule'
  );

  if (prefersReduced) {
    allRevealEls.forEach(el => el.classList.add('is-visible'));
    return;
  }

  const observer = new IntersectionObserver(
    entries => {
      entries.forEach(entry => {
        if (!entry.isIntersecting) return;
        entry.target.classList.add('is-visible');
        observer.unobserve(entry.target);
      });
    },
    { threshold: 0.08, rootMargin: '0px 0px -48px 0px' }
  );

  allRevealEls.forEach(el => observer.observe(el));

  // Section rule: auto-trigger within already-visible sections
  document.querySelectorAll('.section-rule').forEach(rule => {
    const parent = rule.closest('.reveal, .reveal-fade, .is-visible, section');
    if (!parent || parent.classList.contains('is-visible')) {
      rule.classList.add('is-visible');
    }
  });

  // Product card stagger — set --card-delay custom property
  document.querySelectorAll('.products-grid .product-card').forEach((card, i) => {
    card.style.setProperty('--card-delay', (i % 4) * 60 + 'ms');
  });

  // Stagger collection cards
  document.querySelectorAll('.collections-grid .coll-card').forEach((card, i) => {
    if (!card.style.getPropertyValue('--reveal-delay')) {
      card.style.setProperty('--reveal-delay', i * 120 + 'ms');
    }
  });
}
