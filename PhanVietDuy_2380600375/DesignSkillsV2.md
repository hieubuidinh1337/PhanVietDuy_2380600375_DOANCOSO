bash

cat > /tmp/designskills_v2.md << 'HEREDOC'
# VIETDUY DESIGN SKILL v2.0
## Ultra-Luxury Edition · AI Agent Design Reference
### Inspired by: Bottega Veneta · Loro Piana · The Row · Aesop · Dieter Rams

---

> **The single rule above all rules:**
> *"A luxury brand does not compete on features. It competes on feeling."*
> Every pixel decision must answer: **"Does this feel like it costs $10,000?"**
> If not — remove it, not add to it.

---

## WHAT CHANGED FROM v1.0

```
v1.0 → v2.0 UPGRADES:
  + Editorial Motion System   — 14 choreographed animations with purpose
  + Cinematic Landing Page    — 7-section story arc like a fashion film
  + Luxury Micro-interactions — 22 new interaction details
  + Advanced Typography       — optical sizing, editorial hierarchy
  + Immersive Scroll          — parallax, clip-path reveals, pin sections
  + Dark Mode Luxury          — full dark editorial variant
  + Premium Cursor            — custom cursor system
  + Sound Design hints        — when/where to use subtle audio cues
  + 30 new anti-patterns      — things that look cheap on luxury sites
```

---

## 0. PHILOSOPHY UPGRADE

### The 5 Senses of Luxury UI

```
SIGHT     → Editorial restraint. One thing at a time. Hierarchy so clear
            the eye travels without thinking.

TOUCH     → Every interaction has physical weight. Buttons press.
            Drawers glide. Nothing snaps — everything eases.

SOUND     → Silence is the luxury. No notification sounds.
            Optional: 1 subtle UI sound (drawer open, 0.08s, 18kHz).

SMELL     → Metaphorical. The "scent" of the page: warm parchment tones,
            slow reveals, generous whitespace = calm = expensive.

TIME      → Luxury customers are not in a hurry. Never rush them.
            Longer transitions (450ms base, not 280ms) signal confidence.
```

### Reference Brands — Study These

```
THEIR.com         → Ultra-minimal, single typeface, extreme whitespace
Loro Piana        → No logo on hero. Product IS the hero.
The Row           → Black & white, no color, pure form
Aesop             → Dense editorial copy treated as visual element
Bottega Veneta    → Full-bleed imagery, no text over images (just below)
Dieter Rams       → "Less, but better" — every element justifies itself
Rick Owens        → Darkness as a design choice, not default
```

---

## 1. COLOR SYSTEM v2.0

### 1.1 Core Palette — UNCHANGED (correct base)

```css
:root {
  /* All v1.0 variables remain — additions below */

  /* NEW: Film-grain texture layer */
  --c-grain: url("data:image/svg+xml,..."); /* Subtle noise overlay */

  /* NEW: Extended ink scale */
  --c-ink-5:   rgba(26,24,20,0.05);
  --c-ink-10:  rgba(26,24,20,0.10);
  --c-ink-20:  rgba(26,24,20,0.20);
  --c-ink-40:  rgba(26,24,20,0.40);

  /* NEW: Surface variants */
  --c-surface-warm: #FDFAF6;   /* Warmer than pure white — hero cards */
  --c-surface-cool: #F4F2EF;   /* Cooler — form backgrounds            */

  /* NEW: Dark mode (editorial night) */
  --c-dm-bg:      #0F0D0B;     /* Richer than pure black — warmth      */
  --c-dm-surface: #1A1814;     /* Cards on dark bg                     */
  --c-dm-ink:     #F0EDE8;     /* Slightly warm white                  */
  --c-dm-ink-mid: rgba(240,237,232,0.65);
  --c-dm-ink-lt:  rgba(240,237,232,0.38);
  --c-dm-border:  rgba(240,237,232,0.10);
  --c-dm-gold:    #D4AA5A;     /* Gold is brighter in dark mode        */
}
```

### 1.2 NEW Color Rules for v2.0

```
RULE C6 — Image breathing.
  Never place text directly on a photo.
  Acceptable: text BELOW photo, or on a solid color panel adjacent.
  If overlay is required: gradient must be at least 200px tall, max opacity 0.65.

RULE C7 — The One Dark Section Rule.
  One section per page may use --c-bg-dark as background.
  That section must: (a) be at least 400px tall, (b) contain NO product cards,
  (c) serve a storytelling purpose (testimonial, brand story, manifesto).

RULE C8 — Gold is a reward, not decoration.
  Gold (#B8963E) appears a maximum of 3 times per page view.
  Uses: active nav underline, section accent rule, one icon or badge.
  The moment gold is everywhere, it means nothing.

RULE C9 — Hover state = one step darker.
  Background hover: current bg → next darker bg variable.
  --c-bg → --c-bg-alt → --c-surface-cool
  Never jump two steps. Never use a completely different color.

RULE C10 — Image color harmony.
  Editorial images must have warm tones (sunset, golden hour, neutral studio).
  Never use: neon lit images, high-saturation colors, or clinical white backgrounds.
  Unsplash search terms: "editorial fashion warm", "atelier hands craft", 
  "luxury detail macro", "Paris morning light", "linen texture close"
```

---

## 2. TYPOGRAPHY v2.0 — OPTICAL SIZING

### 2.1 The Editorial Type Hierarchy

```
TIER 1 — CINEMATIC (Landing hero only)
  Font:   Cormorant Garamond Italic
  Size:   clamp(5rem, 10vw, 9rem)
  Weight: 300
  Style:  Italic
  Use:    First H1 the user sees on landing page. ONE usage per site.
  Rule:   Never use at this size anywhere else.

TIER 2 — EDITORIAL (Page heroes, section titles)
  Font:   Cormorant Garamond Regular
  Size:   clamp(2.5rem, 5vw, 4.5rem)
  Weight: 300 or 400
  Use:    H1 on interior pages, major section headings.

TIER 3 — CONFIDENT (Product names, card titles)
  Font:   Cormorant Garamond Medium
  Size:   1.25rem – 1.6rem
  Weight: 500
  Use:    Product names, sidebar titles, modal headings.

TIER 4 — FUNCTIONAL (All UI)
  Font:   DM Sans
  Size:   10px – 17px per existing scale
  Weight: 300, 400, 500
  Use:    Everything that is NOT brand voice.
```

### 2.2 NEW Advanced Typography Rules

```
RULE T7 — Optical margin alignment.
  Large display text (TIER 1, 2): apply negative left margin equal to
  the optical indent of the first letter.
  Cormorant "V": margin-left: -0.04em
  Cormorant "L": margin-left: -0.02em
  Cormorant "A": margin-left: 0
  This makes text visually flush with the container edge.

RULE T8 — The editorial aside.
  Short captions beside images use:
    font: DM Sans 11px, weight 300
    color: var(--c-ink-lt)
    writing-mode: vertical-rl (rotated 90°, bottom to top)
    position: absolute, left: -48px
  This is the Bottega Veneta technique — seen in editorial photography.

RULE T9 — Number display.
  Stats, prices, counts: use font-variant-numeric: tabular-nums oldstyle-nums;
  This makes numbers feel hand-typeset rather than digital.

RULE T10 — The long read treatment.
  Brand story sections, product descriptions longer than 80 words:
    font-size: var(--fs-base) — 15px
    line-height: 1.85          — not 1.65
    max-width: 58ch            — slightly narrower than body max
    color: var(--c-ink-mid)    — not full ink
    column-gap: 48px           — if 2-column layout
  Reading comfort signals investment in the reader.

RULE T11 — Eyebrow tags are sacred.
  The small label above a section title ("Collection SS 2025"):
    font: DM Sans
    size: exactly 10px
    weight: 400
    tracking: 0.25em
    case: sentence case (not uppercase, not title case)
    color: var(--c-gold)
  This 3-line combo (eyebrow + display title + rule line) is the
  visual signature of the brand. Never alter proportions.

RULE T12 — Widows and orphans.
  Never allow a single word on the last line of a paragraph.
  Use &nbsp; between the last two words if needed in static copy.
  In dynamic content: CSS hyphens: auto; hyphenate-limit-chars: 6 3 3;
```

---

## 3. SPATIAL SYSTEM v2.0 — PROPORTIONAL BREATHING

### 3.1 The Golden Ratio Section System

```css
/* Sections follow a proportional rhythm, not fixed values */
/* Base unit: 1 "breath" = 96px desktop */

.section { padding-block: calc(var(--breath) * 1); }    /* 96px  */
.section--wide { padding-block: calc(var(--breath) * 1.618); } /* 155px */
.section--hero { padding-block: calc(var(--breath) * 2.618); } /* 251px */

/* These values feel natural because they're derived from φ (phi) */
```

### 3.2 The White Space Manifesto

```
THE LESS IS MORE DOCTRINE:

1. When content looks crowded: remove 1 element, don't reduce padding.
2. Sidebar: maximum 4 elements. If you need a 5th, remove one.
3. Cards: maximum 4 pieces of information visible without hover.
4. Nav: maximum 3 links + 2 action items. If you need more, redesign.
5. Footer: maximum 3 columns × 5 links each. Never paginate the footer.

THE 3-SECOND RULE:
  A new visitor must understand the page purpose in 3 seconds.
  If the hero section needs explanation, it has failed.
  Strip everything that doesn't answer "what is this place?"

THE FRAMING RULE:
  Every content block must have breathing room on all 4 sides.
  Minimum frame: 60px desktop, 24px mobile.
  Images touching the browser edge: intentional full-bleed only, never accidental.
```

### 3.3 NEW Spatial Tokens

```css
:root {
  /* Breathing units */
  --breath:      96px;
  --breath-sm:   48px;
  --breath-xs:   24px;

  /* Prose widths */
  --w-prose-sm:  52ch;   /* Tight editorial               */
  --w-prose-md:  65ch;   /* Comfortable read              */
  --w-prose-lg:  80ch;   /* Max before line length fatigue */

  /* Image aspect ratios (named) */
  --ar-portrait:  3/4;   /* Garment detail shots    */
  --ar-landscape: 16/9;  /* Atelier/location shots  */
  --ar-square:    1/1;   /* Product flat lay        */
  --ar-cinema:    21/9;  /* Hero full-width         */
  --ar-card:      4/3;   /* Product cards (keep)    */
}
```

---

## 4. MOTION SYSTEM v2.0 — CINEMATIC

### 4.1 The Motion Manifesto

```
Animations are not decoration. They are the brand's body language.

FAST animation = cheap, nervous, startup-y.
SLOW animation = confident, assured, expensive.

The luxury baseline is 100ms SLOWER than your instinct.
Your first instinct:      280ms
Luxury baseline:          380ms
Hero cinematic:           600-900ms
Page transitions:         450-600ms

The easing curve is the soul:
  cubic-bezier(0.22, 1, 0.36, 1) = confident arrival
  Objects decelerate to a stop — they don't "bounce" unless intentional.
```

### 4.2 NEW Timing System

```css
:root {
  /* BASE EASES */
  --ease-luxury:   cubic-bezier(0.16, 1, 0.3, 1);   /* Confident. Strong decelerate  */
  --ease-reveal:   cubic-bezier(0.0, 0, 0.2, 1);    /* Content reveals — heavy start */
  --ease-elegant:  cubic-bezier(0.25, 0.1, 0.25, 1);/* Refined, balanced             */
  --ease-spring:   cubic-bezier(0.34, 1.56, 0.64, 1);/* Micro only, max 1 per page   */
  --ease-out:      cubic-bezier(0.22, 1, 0.36, 1);  /* Keep from v1 for compatibility */

  /* DURATIONS — all 100ms slower than v1 */
  --dur-blink:  80ms;    /* State toggle (checkbox, toggle)     */
  --dur-micro:  160ms;   /* Icon swap, color change             */
  --dur-fast:   260ms;   /* Hover transitions                   */
  --dur-base:   380ms;   /* Card lifts, drawer triggers         */
  --dur-slow:   550ms;   /* Modal open, overlay fade            */
  --dur-reveal: 700ms;   /* Scroll-reveal, section entrance     */
  --dur-hero:   900ms;   /* Hero entrance (one-time)            */
  --dur-cinema: 1200ms;  /* Full cinematic transitions          */
}
```

### 4.3 The 14 Choreographed Animations

```css
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 1. HERO ENTRANCE — first impression, one-time  */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
@keyframes heroImageReveal {
  from { clip-path: inset(0 100% 0 0); }
  to   { clip-path: inset(0 0% 0 0); }
}
/* Reveal image left-to-right like a curtain. 900ms. */

@keyframes heroTextFade {
  from { opacity: 0; transform: translateY(16px); }
  to   { opacity: 1; transform: translateY(0); }
}
/* Text fades up. 600ms, delay 300ms after image starts. */

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 2. SCROLL REVEAL — editorial content entrance  */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.reveal-clip {
  clip-path: inset(0 0 100% 0);
  transition: clip-path var(--dur-reveal) var(--ease-reveal);
}
.reveal-clip.is-visible { clip-path: inset(0 0 0% 0); }
/* Bottom edge travels upward revealing content — like raising a curtain from below */

.reveal-fade {
  opacity: 0;
  transform: translateY(28px);
  transition:
    opacity   var(--dur-reveal) var(--ease-luxury),
    transform var(--dur-reveal) var(--ease-luxury);
}
.reveal-fade.is-visible { opacity: 1; transform: translateY(0); }

.reveal-rule {
  width: 0;
  transition: width var(--dur-reveal) var(--ease-luxury);
}
.reveal-rule.is-visible { width: 44px; }
/* Gold accent rule draws itself when section enters viewport */

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 3. CARD HOVER — the luxury lift                */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.product-card {
  transition:
    transform    var(--dur-base) var(--ease-luxury),
    box-shadow   var(--dur-base) var(--ease-luxury),
    border-color var(--dur-base) var(--ease-luxury);
}
.product-card:hover {
  transform: translateY(-10px);
  box-shadow:
    0 2px 4px rgba(26,24,20,.04),
    0 8px 16px rgba(26,24,20,.06),
    0 24px 48px rgba(26,24,20,.10),
    0 48px 80px rgba(26,24,20,.06);
    /* Layered shadow = physical depth, not flat */
  border-color: var(--c-gold-lt);
}
.product-card__img {
  transition: transform var(--dur-slow) var(--ease-luxury);
}
.product-card:hover .product-card__img { transform: scale(1.06); }

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 4. NAV LINK UNDERLINE — brand signature        */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.nav__link::after {
  transform-origin: left center;
  width: 100%; height: 1px;
  background: var(--c-gold);
  transform: scaleX(0);
  transition: transform var(--dur-fast) var(--ease-luxury);
}
.nav__link:hover::after,
.nav__link--active::after { transform: scaleX(1); }
/* scaleX is smoother than width animation — no layout reflow */

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 5. IMAGE GALLERY — surgical crossfade          */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.gallery__img-enter {
  animation: galleryCross var(--dur-slow) var(--ease-luxury) forwards;
}
@keyframes galleryCross {
  from { opacity: 0; transform: scale(1.02); }
  to   { opacity: 1; transform: scale(1); }
}
/* Slight zoom-out on entry = feels like a camera focus pull */

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 6. CART DRAWER — silk slide                    */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.cart-drawer {
  transform: translateX(100%);
  transition: transform var(--dur-slow) var(--ease-luxury);
}
.cart-drawer.is-open { transform: translateX(0); }
.cart-scrim {
  opacity: 0;
  transition: opacity var(--dur-slow) var(--ease-elegant);
}
.cart-scrim.is-visible { opacity: 1; }

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 7. BUTTON PRESS — physical feedback            */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.btn { transition: transform var(--dur-micro) var(--ease-luxury), all var(--dur-fast) var(--ease-luxury); }
.btn:active { transform: scale(0.98) translateY(1px); }

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 8. SEARCH OVERLAY — elegant expansion          */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.search-overlay {
  max-height: 0; overflow: hidden;
  transition: max-height var(--dur-base) var(--ease-luxury);
}
.search-overlay.is-open { max-height: 72px; }
.search-overlay input {
  width: 0;
  transition: width var(--dur-slow) var(--ease-luxury) 100ms;
}
.search-overlay.is-open input { width: 100%; }

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 9. SECTION TRANSITIONS (SPA-like MVC pages)   */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* Apply with AJAX partial navigation if available */
.page-transition-out {
  animation: pageOut var(--dur-fast) var(--ease-reveal) forwards;
}
@keyframes pageOut {
  to { opacity: 0; transform: translateY(-12px); }
}
.page-transition-in {
  animation: pageIn var(--dur-reveal) var(--ease-luxury) forwards;
}
@keyframes pageIn {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 10. FILTER PILL — active state snap            */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.filter-pill { transition: all var(--dur-fast) var(--ease-luxury); }
.filter-pill--active {
  animation: pillLock 300ms var(--ease-luxury);
}
@keyframes pillLock {
  0%   { transform: scale(0.95); }
  60%  { transform: scale(1.02); }
  100% { transform: scale(1); }
}

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 11. WISH BUTTON — heart fill reveal            */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.product-card__wish i {
  transition: transform var(--dur-micro) var(--ease-spring);
}
.product-card__wish.is-active i { transform: scale(1.2); }
@keyframes heartPop {
  0%   { transform: scale(1); }
  40%  { transform: scale(1.35); }
  70%  { transform: scale(0.9); }
  100% { transform: scale(1); }
}
.product-card__wish.just-activated i { animation: heartPop 400ms var(--ease-spring); }

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 12. ACCORDION — silk open                     */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.accordion__content {
  display: grid;
  grid-template-rows: 0fr;
  transition: grid-template-rows var(--dur-base) var(--ease-luxury);
}
.accordion__content.is-open { grid-template-rows: 1fr; }
.accordion__content > div { overflow: hidden; }
/* grid-template-rows trick = smooth without fixed max-height */

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 13. TOAST NOTIFICATION — confident entry      */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
@keyframes toastIn {
  from { opacity: 0; transform: translateY(16px) scale(0.96); }
  to   { opacity: 1; transform: translateY(0) scale(1); }
}
@keyframes toastOut {
  to { opacity: 0; transform: translateY(8px) scale(0.98); }
}
.toast { animation: toastIn var(--dur-base) var(--ease-luxury); }
.toast.is-leaving { animation: toastOut var(--dur-fast) var(--ease-reveal) forwards; }

/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
/* 14. STAGGER REVEAL — product grid entrance    */
/* ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ */
.product-card { opacity: 0; animation: cardEnter var(--dur-reveal) var(--ease-luxury) both; }
@keyframes cardEnter {
  from { opacity: 0; transform: translateY(24px); }
  to   { opacity: 1; transform: translateY(0); }
}
/* Delay via CSS custom property set by JS */
.product-card { animation-delay: var(--card-delay, 0ms); }
/* JS: card.style.setProperty('--card-delay', i * 60 + 'ms'); */
```

---

## 5. CUSTOM CURSOR SYSTEM

```css
/* Luxury sites use custom cursors as a signature detail */

body { cursor: none; } /* Hide default cursor entirely */

/* Custom cursor: 8px circle, tracks mouse */
.cursor-dot {
  position: fixed; top: 0; left: 0; z-index: 9999;
  width: 8px; height: 8px; border-radius: 50%;
  background: var(--c-ink);
  pointer-events: none;
  transform: translate(-50%, -50%);
  transition: transform 80ms var(--ease-luxury),
              width 200ms var(--ease-luxury),
              height 200ms var(--ease-luxury),
              background 200ms var(--ease-luxury);
  mix-blend-mode: difference; /* Inverts on light/dark surfaces */
}

/* Hover states — cursor morphs based on target */
body:has(a:hover) .cursor-dot,
body:has(button:hover) .cursor-dot { transform: translate(-50%,-50%) scale(3); }

body:has(.product-card:hover) .cursor-dot {
  width: 60px; height: 60px;
  background: transparent;
  border: 1px solid var(--c-ink);
  /* Turns into a circle "viewfinder" on product hover */
}

body:has(.product-card__img:hover) .cursor-dot::after {
  content: 'Xem'; font-size: 9px; color: var(--c-ink);
  position: absolute; top: 50%; left: 50%;
  transform: translate(-50%,-50%);
}

/* Trailing cursor ring: 32px circle, slower follow */
.cursor-ring {
  position: fixed; top: 0; left: 0; z-index: 9998;
  width: 32px; height: 32px; border-radius: 50%;
  border: 1px solid rgba(26,24,20,0.3);
  pointer-events: none;
  transform: translate(-50%,-50%);
  /* Lags behind cursor = expensive trailing effect */
  /* Implemented via requestAnimationFrame lerp in JS */
}

/* JS implementation:
let mouseX = 0, mouseY = 0, ringX = 0, ringY = 0;
document.addEventListener('mousemove', e => { mouseX = e.clientX; mouseY = e.clientY; });
function animateCursor() {
  const dot = document.querySelector('.cursor-dot');
  const ring = document.querySelector('.cursor-ring');
  dot.style.left = mouseX + 'px';
  dot.style.top  = mouseY + 'px';
  ringX += (mouseX - ringX) * 0.12;  // lerp factor = lag
  ringY += (mouseY - ringY) * 0.12;
  ring.style.left = ringX + 'px';
  ring.style.top  = ringY + 'px';
  requestAnimationFrame(animateCursor);
}
animateCursor();
*/

/* Mobile: restore default cursor */
@media (hover: none) {
  body { cursor: auto; }
  .cursor-dot, .cursor-ring { display: none; }
}
```

---

## 6. LANDING PAGE — CINEMATIC 7-ACT STRUCTURE

```
The landing page is a film. It has acts, not sections.
The user does not "scroll a page" — they experience a story.

ACT 1  — THE OPENING SHOT    (Hero — full viewport)
ACT 2  — THE WORLD           (Collections — editorial grid)
ACT 3  — THE HISTORY         (Brand story — split with image)
ACT 4  — THE CRAFT           (Featured products — curated 4)
ACT 5  — THE VOICE           (Testimonial — dark section)
ACT 6  — THE INVITATION      (Newsletter — gold background, 1 per site)
ACT 7  — THE FAREWELL        (Footer — dark, unhurried)

Scroll path:
  User enters → hooked by hero image (ACT 1)
  Discovers product universe (ACT 2)
  Learns who made this (ACT 3)
  Sees what to buy (ACT 4)
  Trusts via social proof (ACT 5)
  Invited into the inner circle (ACT 6)
  Exits with brand feeling intact (ACT 7)
```

### ACT 1 — THE OPENING SHOT (Hero)

```
LAYOUT:
  Full viewport (100svh). Not 80vh. Not 90vh. Exactly 100svh.
  Two columns: text left (40%), image right (60%).
  NO padding-top — content fills the full space.

THE IMAGE (right column):
  Full-bleed editorial photo. Must be:
  - Minimum 1400×1800px
  - Warm-toned, natural light
  - Subject not centered — rule of thirds
  - Never stock-photo-obvious (no perfect smiles, no pointing)
  
  Image enters via clip-path reveal:
  clip-path: inset(0 100% 0 0) → inset(0 0% 0 0)
  Duration: 900ms, ease: var(--ease-luxury)
  Delay: 0ms — IMAGE FIRST, then text

THE TEXT (left column):
  Padding: 80px left edge, centered vertically
  
  Element 1 — Eyebrow (enters at 300ms delay):
    "Collection SS 2025"
    font: DM Sans 10px, tracking 0.28em, color --c-gold
    
  Element 2 — Cinematic H1 (enters at 400ms delay):
    Line 1: "Viet Duy's" — Cormorant 300 normal
    Line 2: "Collection" — Cormorant 300 ITALIC (the italic is the emotion)
    
    SIZING: clamp(4.5rem, 9vw, 8rem)
    WHY THIS SIZE: At 8rem, each letter is 128px tall. This is physically large.
    It says: "We are not whispering. We know our worth."
    
    OPTICAL ALIGNMENT: Apply margin-left: -0.06em to align "V" optically
    
  Element 3 — Subheadline (enters at 550ms delay):
    MAX 2 lines. MAX 40 characters.
    If you cannot describe the brand in 40 characters, you haven't understood it.
    Example: "Nghề thủ công tinh xảo. Kể từ 1987."
    
  Element 4 — CTA group (enters at 700ms delay):
    Button 1: btn--solid "Khám phá bộ sưu tập"
    Button 2: btn--ghost "Câu chuyện của chúng tôi"
    
    IMPORTANT: BTN 2 IS GHOST, NOT OUTLINE.
    Ghost button = lower visual weight = does not compete with primary CTA.
    
  Element 5 — Stats (enters at 900ms delay):
    3 numbers, bottom of left column, pushed to bottom via margin-top: auto
    Number format: font-variant-numeric: oldstyle-nums tabular-nums
    
  Element 6 — Scroll indicator (position: absolute, bottom 32px, left 80px):
    NOT text "Cuộn xuống" — this is dated.
    INSTEAD: Single animated line, 1px wide, 48px tall, --c-gold color
    Animates: height 0 → 48px → 0 (loop, 2s, ease-in-out)
    This is silent, elegant. Let the line speak.

FLOATING EDITORIAL BADGE (position: absolute, bottom 40px, right 48px):
  This is new in v2.0.
  A small panel that appears at 1100ms delay:
  Content: Year + City — "1987 · Paris"
  Style: bg rgba(247,245,242,0.9), backdrop-filter: blur(16px)
         border: 1px solid var(--c-border)
         padding: 12px 20px
         font: DM Sans 11px tracking 0.15em
  This badge whispers context without demanding attention.

MOBILE (< 768px):
  Stack vertically: image (50vh, full-width) → text
  Reduce H1 to clamp(2.8rem, 8vw, 4rem)
  Stats hide on mobile (keep only 2 CTAs)
```

### ACT 2 — THE WORLD (Collections)

```
HEADING BLOCK:
  Left-aligned, never centered.
  Eyebrow: "Khám phá"
  Title: "Danh mục nổi bật" — note: NOT italic here. Italic is for emotions.
         This is information. Use Cormorant Regular.
  Gold rule: animate width 0 → 44px when section enters viewport.

THE GRID — Masonry-editorial hybrid:
  Desktop: 3 columns with unequal heights
    Column 1 (leftmost): aspect-ratio 3/4 — TALL. First impression.
    Column 2 (center):   aspect-ratio 1/1 — SQUARE.
    Column 3 (right):    aspect-ratio 5/4 — MEDIUM.
  
  Gap: 2px — Editorial "bleed" effect. Columns nearly touch.
  This is the Bottega Veneta editorial grid technique.
  
  Each card:
    Image: full area, object-fit: cover, overflow: hidden
    Hover: image scale(1.04) — 600ms, ease-luxury
    
    Text panel: position absolute, bottom 0, full width
      Gradient: rgba(26,24,20,0) top → rgba(26,24,20,0.72) bottom
      Height: 180px
      Content:
        - Tag: "01 — Signature" [10px, tracking 0.2em, rgba(255,255,255,0.6)]
        - Name: Cormorant Italic, 1.8rem, #fff
        - Link arrow: "→" appears on hover, slide in from left (translateX(-8px) → 0)
    
    ENTER ANIMATION: clip-path: inset(0 0 100% 0) → inset(0 0 0% 0)
    Each card enters bottom-up, staggered 120ms apart.
```

### ACT 3 — THE HISTORY (Brand Story)

```
LAYOUT: Full-width, no container max-width on the image.
  Left 45%: image — full-bleed, no margin
  Right 55%: content — padded 80px

THE IMAGE:
  Subject: craftsman hands working leather, or atelier interior
  Must have natural, warm light
  No smile, no eye contact with camera
  aspect-ratio: unset — fills the entire column height

  ANIMATED REVEAL: clip-path: inset(100% 0 0 0) → inset(0 0 0 0)
  Direction: rises from bottom (curtain up)
  Duration: 900ms, trigger: Intersection Observer

THE EDITORIAL ASIDE:
  Position: absolute, left: -40px, middle of image
  Content: Year or city name in vertical text
  Style: writing-mode: vertical-rl, rotate: 180deg
         DM Sans 10px, tracking 0.25em, color var(--c-ink-lt)
  This is the v2.0 detail that makes designers pause.

CONTENT:
  Eyebrow: "Notre Histoire" — use French. Intentional brand bilingualism.
  
  BLOCKQUOTE — The centerpiece of this section:
    font: Cormorant Italic, clamp(1.5rem, 3vw, 2.2rem), weight 300
    border-left: 2px solid var(--c-gold) — the ONLY element using gold in this section
    padding-left: 32px
    line-height: 1.5
    
    Max 3 lines. Every word earns its place.
    
  Body text: DM Sans 15px, line-height 1.85 (the long-read treatment)
  Max 80 words. If you need more, you're explaining, not storytelling.
  
  CTA: btn--ghost (not btn--outline) "Tìm hiểu thêm"
  
  DECORATIVE YEAR:
    position: absolute, right: 40px, bottom: 40px
    font: Cormorant, 10rem, weight 300, color: var(--c-ink-5)
    pointer-events: none; user-select: none
    This creates depth — a ghost behind the content.
```

### ACT 4 — THE CRAFT (Featured Products)

```
HEADER: flex row, space-between, align-items: flex-end
  (title on left, "Xem tất cả →" link on right)
  
  The right-aligned link is important: it communicates "there is more."
  This creates curiosity without being salesy.

PRODUCT GRID:
  4 products, auto-fill, minmax(260px, 1fr)
  Stagger delay: each card +60ms
  
  ON MOBILE: Horizontal scroll carousel
    display: flex; overflow-x: auto; scroll-snap-type: x mandatory
    Each card: scroll-snap-align: start; min-width: 280px
    Hide scrollbar but keep scrollable
    Show "scroll hint": partial card visible at right edge

THE EMPTY TESTIMONIAL GAP:
  After the 4 products, before the next section:
  Add a single centered italic line:
  "— Được chế tác bởi bàn tay người thợ thủ công Ý —"
  font: Cormorant Italic 15px, color var(--c-ink-lt)
  This micro-copy transition between sections feels like editorial layout.
```

### ACT 5 — THE VOICE (Testimonial)

```
BACKGROUND: var(--c-bg-dark) — the ONLY dark section on homepage.
  This tonal shift creates a visual "act break" — like a scene change.

LAYOUT: Centered, max-width 760px, padding 120px 60px

DECORATIVE QUOTE MARK:
  NOT " " characters. Use the CSS content technique:
  ::before { content: '\201C'; font-family: var(--ff-display);
             font-size: 10rem; line-height: 0.7; color: var(--c-gold);
             opacity: 0.3; display: block; margin-bottom: -40px; }
  This enormous ghost quote mark sits BEHIND the text.

TESTIMONIAL QUOTE:
  Cormorant Italic, clamp(1.3rem, 3vw, 2rem), weight 300
  color: var(--c-dm-ink)
  line-height: 1.65
  Max 40 words. The quote must be specific, not generic.
  BAD:  "Great products, I love them!"
  GOOD: "Mỗi lần cầm chiếc túi Riviera, tôi biết mình đang giữ một tác phẩm."

ATTRIBUTION LINE:
  Thin line: 40px, 1px, --c-gold, centered, margin-bottom 16px
  Text: DM Sans, 12px, tracking 0.1em, rgba(247,245,242,0.5)
  Format: "— [Name] · [Title]"

STARS:
  5 FA fa-star solid icons, color: --c-gold, font-size: 11px
  Display BELOW the attribution, not above
  Stars below = confidence. Stars above = desperation for validation.
```

### ACT 6 — THE INVITATION (Newsletter)

```
BACKGROUND: var(--c-gold) — only gold background on site.
  Used once = sacred. Used twice = decoration.

This entire section should feel warm, not corporate.

HEADING: Cormorant Regular, 2.5rem, color #fff
  "Trở thành người đầu tiên biết"
  NOT "Đăng ký nhận tin" — that sounds like a checkbox.
  Frame it as exclusivity, not subscription.

SUBTEXT: DM Sans 15px, rgba(255,255,255,0.8)
  "Bộ sưu tập mới. Ra mắt độc quyền. Chỉ dành cho thành viên."

FORM: Inline, centered, max-width 480px
  Single input: email
  Single button: "Tham gia"
  
  Input styling on gold background:
    bg: rgba(255,255,255,0.15)
    border: 1px solid rgba(255,255,255,0.3)
    color: #fff; placeholder: rgba(255,255,255,0.6)
    :focus → bg: rgba(255,255,255,0.25), border: rgba(255,255,255,0.6)
  
  Button: bg rgba(26,24,20,0.85), color #fff
    :hover → bg var(--c-ink)
  
  Below form: "Không có spam. Hủy đăng ký bất cứ lúc nào."
  DM Sans 11px, rgba(255,255,255,0.5)
  This micro-copy removes the subscription anxiety.
```

---

## 7. NEW COMPONENT: MARQUEE STRIP

```
Between ACT 2 and ACT 3, add a horizontal scrolling marquee:
Purpose: signals brand values without demanding attention.

Content: rotating text items separated by "·"
"Thủ công tại Paris  ·  Chứng nhận GIA  ·  38 năm kinh nghiệm  ·  Giao hàng toàn cầu  ·"

CSS:
.marquee {
  overflow: hidden;
  border-block: 1px solid var(--c-border);
  padding: 14px 0;
  background: var(--c-bg-alt);
}
.marquee__track {
  display: flex; gap: 64px;
  width: max-content;
  animation: marqueeScroll 30s linear infinite;
}
.marquee__track:hover { animation-play-state: paused; }
@keyframes marqueeScroll {
  from { transform: translateX(0); }
  to   { transform: translateX(-50%); }
}
.marquee__item {
  font: DM Sans 11px/1 400;
  letter-spacing: 0.15em;
  text-transform: uppercase;
  color: var(--c-ink-lt);
  white-space: nowrap;
}
/* Duplicate items in HTML for seamless loop */
```

---

## 8. NEW COMPONENT: EDITORIAL IMAGE CAPTION

```
For the brand story section and product detail, add aside captions:

HTML:
<figure class="editorial-figure">
  <img ... />
  <figcaption class="editorial-caption">Xưởng thủ công tại Florence, Ý</figcaption>
</figure>

CSS:
.editorial-figure { position: relative; }
.editorial-caption {
  position: absolute;
  left: -56px; top: 50%; transform: translateY(-50%);
  writing-mode: vertical-rl; rotate: 180deg;
  font: var(--fs-xs)/1 var(--ff-body);
  font-size: 10px; font-weight: 300;
  letter-spacing: 0.15em;
  color: var(--c-ink-lt);
  white-space: nowrap;
}
@media (max-width: 1024px) {
  .editorial-caption { display: none; } /* Too tight on tablet */
}
```

---

## 9. NEW COMPONENT: PRICE REVEAL ANIMATION

```
When user hovers over a product card, price transitions:
$890 → $890.00 (decimal appears) and slight gold color.

CSS:
.product-card__price {
  transition: color var(--dur-fast) var(--ease-luxury);
}
.product-card:hover .product-card__price {
  color: var(--c-gold-dk);
}

For the ".00" appearing: use a span with overflow:hidden + width 0 → auto
.price-cents {
  display: inline-block; overflow: hidden;
  max-width: 0;
  transition: max-width var(--dur-base) var(--ease-luxury);
  vertical-align: baseline;
}
.product-card:hover .price-cents { max-width: 40px; }
```

---

## 10. DARK MODE — EDITORIAL NIGHT

```
Media query: @media (prefers-color-scheme: dark)
Also: [data-theme="dark"] for JS toggle

.dark-theme {
  --c-bg:         var(--c-dm-bg);
  --c-bg-alt:     #181410;
  --c-surface:    var(--c-dm-surface);
  --c-ink:        var(--c-dm-ink);
  --c-ink-mid:    var(--c-dm-ink-mid);
  --c-ink-lt:     var(--c-dm-ink-lt);
  --c-border:     var(--c-dm-border);
  --c-border-s:   rgba(240,237,232,0.20);
  --c-gold:       var(--c-dm-gold);   /* Brighter in dark */
}

Dark mode rules:
- Footer dark section becomes the PRIMARY background — loses distinction.
  Solution: footer background in dark mode = #000 (true black), 
  creating contrast against #0F0D0B body.
- Product card box-shadow in dark mode:
  0 24px 64px rgba(0,0,0,.6) — much stronger shadow needed.
- Image brightness: filter: brightness(0.9) in dark mode.
  Images shot for light environments look slightly washed in dark.
```

---

## 11. ADDITIONAL ANTI-PATTERNS v2.0

```
30 NEW THINGS THAT MAKE LUXURY UI LOOK CHEAP:

MACRO SINS:
  ✗ Hero section without a dominant image — text-only heroes look like blogs
  ✗ Product count text ("13 sản phẩm") in a badge/pill — use plain text only
  ✗ Pagination as numbered buttons — use "Xem thêm" single link
  ✗ Breadcrumb on homepage — not needed, removes clean entry
  ✗ Social proof numbers ("10,000 happy customers") — feels like Shopify
  ✗ Countdown timers of any kind — urgency is anti-luxury
  ✗ "Free shipping on orders over X" banner above nav — noise
  ✗ Cookie consent modal over hero on first load — block with elegant banner below
  ✗ Chat bubble widget (Intercom etc.) on luxury brand — breaks editorial feel
  ✗ Progress bars on forms — signals transaction, not relationship

TYPOGRAPHY SINS:
  ✗ Text shadows on any text ever — even on image overlays
  ✗ Outline/stroked text — startup/streetwear aesthetic
  ✗ Mix of serif and sans-serif IN THE SAME HEADING — pick one
  ✗ Decorative initial caps (drop caps) — editorial, not luxury
  ✗ Testimonial quotation marks styled as large decorated icons
  ✗ Author name in quotes before the quote text — attribution is below
  ✗ Bullet points in product descriptions — use prose
  ✗ Numbered steps for "how it works" — use narrative

COLOR SINS:
  ✗ Gold and another accent color on same page — pick gold OR the other
  ✗ Frosted glass (backdrop-filter: blur) overused — max 2 elements
  ✗ Colored footer links (using brand color) — all footer links are muted
  ✗ Colored background on the entire main nav — nav must be transparent or white
  ✗ Any red except for danger/error states — never decorative red
  ✗ "Navy + gold" combo — it reads nautical, not Parisian luxury

INTERACTION SINS:
  ✗ Click audio feedback on buttons (except very deliberate brand sound)
  ✗ Particle effects on hero (floating gold dust etc.) — cheap
  ✗ Parallax on mobile — causes scroll jank, ruins the experience
  ✗ Auto-playing video with sound — never
  ✗ Popups that appear while user is reading — reading is sacred
  ✗ "Back to top" button that appears too early (< 300px scroll)
  ✗ Loading skeleton screens — prefer opacity fade, smoother
  ✗ Hover states on mobile devices — they "stick" on touch, confusing
```

---

## 12. PERFORMANCE AS LUXURY

```
A slow website is a cheap website.
Target metrics for Vietduy:
  LCP (Largest Contentful Paint): < 2.0s
  CLS (Cumulative Layout Shift):  < 0.05
  FID / INP:                      < 100ms
  
TECHNIQUES:
  1. Hero image: preload via <link rel="preload" as="image"> in <head>
  2. All non-hero images: loading="lazy" + explicit width/height
  3. Fonts: font-display: swap on @font-face
  4. CSS: no @import chains — all in <link> tags
  5. JS: all scripts defer or type="module"
  6. Images: WebP format, serve via <picture> with JPEG fallback
  7. No animation on scroll events — use Intersection Observer only

<picture>
  <source srcset="/images/hero.webp" type="image/webp" />
  <img src="/images/hero.jpg" alt="..." width="1400" height="1867"
       fetchpriority="high" decoding="async" />
</picture>

Font preload (critical, above fold only):
<link rel="preload" href="/fonts/CormorantGaramond-Regular.woff2"
      as="font" type="font/woff2" crossorigin />
```

---

## 13. LANDING PAGE CHECKLIST — "IS IT LUXURY?"

```
Run through this before considering the page done:

FIRST FOLD (no scroll):
□ Does the hero image take over the right side completely?
□ Is the H1 physically large (≥ 64px equivalent)?
□ Is there exactly ONE primary CTA?
□ Is there NO noise above or below the nav?
□ Does the hero feel complete without scrolling?

TYPOGRAPHY:
□ Does every element use ONLY --ff-display or --ff-body?
□ Is --c-gold used max 3 times in the entire page?
□ Is ALL prose in sentence case?
□ Does the page have zero ALL CAPS (outside brand name)?
□ Are product names NOT uppercased?

MOTION:
□ Does the hero image enter via clip-path reveal (not fade)?
□ Do section reveals use Intersection Observer, not scroll events?
□ Are all transitions ≥ 350ms?
□ Is there 0 looping decorative animation (except marquee, scroll line)?
□ Does every hover state have a clear physical metaphor (lift, reveal, grow)?

SPACING:
□ Is there min 96px between every major section on desktop?
□ Do all card internals have at least 20px padding?
□ Are all image aspect ratios consistent within their section?
□ Is there a minimum 60px frame on left and right of all content?

COPY:
□ Is hero subheadline ≤ 40 characters?
□ Are all CTAs action verbs ("Khám phá", not "Tìm hiểu")?
□ Is testimonial ≤ 40 words?
□ Does newsletter copy say "exclusivity" not "subscription"?
□ Are all error messages in human Vietnamese (not technical)?

THE FEEL TEST:
□ Close your eyes. Think: "Does this feel like it costs $10,000?"
□ If yes: ship it.
□ If no: remove something, don't add something.
```

---

*VIETDUY DESIGN SKILL v2.0 — Ultra-Luxury Edition*
*"Luxury is not a price point. It is a state of mind made visible."*
*When in doubt: subtract, don't add. Then subtract again.*
HEREDOC

echo "v2 skill: $(wc -l < /tmp/designskills_v2.md) lines