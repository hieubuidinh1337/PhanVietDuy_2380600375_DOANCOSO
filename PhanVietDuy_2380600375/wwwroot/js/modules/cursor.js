/**
 * cursor.js — Luxury lerp-based trailing cursor (v2.0)
 * DesignSkillsV2.md Section 5
 * Only active on desktop (hover: hover devices).
 */
export function initCursor() {
  // Skip on touch devices
  if (window.matchMedia('(hover: none)').matches) return;

  const dot  = document.querySelector('.cursor-dot');
  const ring = document.querySelector('.cursor-ring');
  if (!dot || !ring) return;

  let mouseX = 0, mouseY = 0;
  let ringX  = 0, ringY  = 0;

  // Track mouse position (dot follows immediately)
  document.addEventListener('mousemove', e => {
    mouseX = e.clientX;
    mouseY = e.clientY;
    dot.style.left = mouseX + 'px';
    dot.style.top  = mouseY + 'px';
  }, { passive: true });

  // Ring trails behind using lerp
  function tick() {
    ringX += (mouseX - ringX) * 0.10;
    ringY += (mouseY - ringY) * 0.10;
    ring.style.left = ringX + 'px';
    ring.style.top  = ringY + 'px';
    requestAnimationFrame(tick);
  }
  tick();

  // Show cursor (hidden by default to avoid flash)
  dot.style.opacity  = '1';
  ring.style.opacity = '1';
}
