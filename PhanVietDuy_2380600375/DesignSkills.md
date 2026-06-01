# VIETDUY DESIGN SKILL

## AI Agent Design Reference · Luxury Atelier UI System


---

> **How to use this file**  
> This file loads before every UI task. Read it entirely before writing a single line of HTML, CSS, or Razor.  
> Every decision — color, spacing, font size, motion, copy — must trace back to a rule here.  
> When in doubt: **subtract, don't add.**

---

## 0. BRAND IDENTITY

```
Brand name   : Vietduy Atelier
Founded      : 1987, Paris
Tagline      : "L'art de vivre luxe"
Archetype    : The Sage × The Ruler — knowledgeable, refined, authoritative, never flashy
Voice        : Editorial. Confident but never arrogant. French-inflected Vietnamese.
Audience     : High-net-worth individuals, 28–55, bilingual (VI/FR/EN), aesthetically educated
Anti-persona : Anyone impressed by loud logos, discount badges, or "BUY NOW" urgency
```

**Brand personality spectrum:**

```
Minimal ←————————————●———→ Ornate
Quiet   ←——————●——————————→ Loud
Timeless←————————————●———→ Trendy
Digital ←—●———————————————→ Artisanal
```

---

## 1. COLOR SYSTEM

### 1.1 Core Palette (CSS Custom Properties)

```css
:root {
  /* ── Backgrounds ── */
  --c-bg: #f7f5f2; /* Warm parchment — primary surface      */
  --c-bg-alt: #eeeae3; /* Slightly deeper warm — hover, alt rows */
  --c-bg-dark: #1a1814; /* Near-black — footer, overlays          */
  --c-surface: #ffffff; /* Pure white — cards, modals             */
  --c-surface-muted: #f2efe9; /* Muted card — secondary cards           */

  /* ── Ink (Text) ── */
  --c-ink: #1a1814; /* Primary text — near-black, warm        */
  --c-ink-mid: #4a463f; /* Secondary text — body copy             */
  --c-ink-lt: #8c877e; /* Tertiary — captions, labels, metadata  */
  --c-ink-faint: #c4beb5; /* Disabled, placeholder                  */

  /* ── Accent ── */
  --c-gold: #b8963e; /* Champagne gold — PRIMARY accent        */
  /* Contrast on --c-bg: 4.6:1 ✓ WCAG AA   */
  --c-gold-lt: #e8d5a3; /* Light gold — tints, badges             */
  --c-gold-dk: #8c6e28; /* Dark gold — hover state on gold        */

  /* ── Borders ── */
  --c-border: #e4ded6; /* Default border — cards, dividers       */
  --c-border-s: #c8bfb4; /* Strong border — inputs focus, emphasis */

  /* ── Semantic ── */
  --c-success: #3b6d11;
  --c-warning: #854f0b;
  --c-danger: #a32d2d;
  --c-info: #185fa5;
}
```

> ⚠️ **IMPORTANT:** `--c-gold` was corrected from `#C6A96A` (3.8:1, fails AA) to `#B8963E` (4.6:1, passes AA).  
> Never use the old value. Every text on `--c-bg` using gold must use `#B8963E` or darker.

### 1.2 Color Rules

```
RULE 1 — One accent.
  Use --c-gold for exactly one role per component: either the icon, the border,
  OR the text — never all three simultaneously.

RULE 2 — No rainbow.
  The palette has no blue, red, or green UI chrome. Only semantic uses (success/danger/info).
  If you feel the urge to add color, add whitespace instead.

RULE 3 — Dark surfaces use light text only.
  On --c-bg-dark (#1A1814): use #F7F5F2 for primary text, rgba(247,245,242,0.55) for secondary.
  Never use --c-ink or --c-ink-mid on dark backgrounds.

RULE 4 — Overlays.
  Image overlays: rgba(26,24,20, 0.0) → rgba(26,24,20, 0.55) gradient from top to bottom.
  Modal scrim: rgba(26,24,20, 0.7).

RULE 5 — Badges.
  "New"       → bg: --c-ink,   text: --c-bg,    9px uppercase
  "Limited"   → bg: --c-gold,  text: #fff,       9px uppercase
  "Exclusive" → bg: --c-gold,  text: #fff,       9px uppercase
  "Sale"      → bg: --c-danger,text: #fff,       9px uppercase
  Never use colored backgrounds for anything else.
```

### 1.3 What NOT to do with color

```
✗ Gradient text (background-clip: text) — flagged by Impeccable detector
✗ Box shadows with color (e.g. box-shadow: 0 4px 20px rgba(198,169,106,0.4))
✗ Neon, glow, bloom effects of any kind
✗ Pure black (#000000) anywhere — use --c-ink (#1A1814)
✗ Pure white (#FFFFFF) as page background — use --c-bg (#F7F5F2)
✗ More than 2 accent uses per component
```

---

## 2. TYPOGRAPHY

### 2.1 Font Stack

```css
--ff-display: "Cormorant Garamond", "EB Garamond", Georgia, serif;
--ff-body: "DM Sans", "Inter", system-ui, sans-serif;
```

> **Why these two:**  
> Cormorant Garamond carries editorial weight — use for brand voice moments.  
> DM Sans is neutral, highly legible — use for all functional UI.  
> Never mix a third typeface.

### 2.2 Type Scale

```css
/* Display — only for hero, section titles */
--fs-display-xl: clamp(3.5rem, 7vw, 6rem); /* Hero H1         */
--fs-display-lg: clamp(2.2rem, 4vw, 3.5rem); /* Page hero H1    */
--fs-display-md: clamp(1.6rem, 3vw, 2.5rem); /* Section titles  */
--fs-display-sm: 1.4rem; /* Card titles, H3 */

/* UI — body, labels, captions */
--fs-lg: 1.0625rem; /* 17px — lead body text       */
--fs-base: 0.9375rem; /* 15px — body copy (default)  */
--fs-sm: 0.8125rem; /* 13px — secondary, captions  */
--fs-xs: 0.6875rem; /* 11px — labels, badges, tags */
--fs-2xs: 0.625rem; /*  10px — legal, timestamps   */
```

### 2.3 Typography Rules

```
RULE T1 — Display font = brand voice only.
  Cormorant Garamond is used ONLY for:
    - H1 page titles
    - H2 section headings
    - Product names in cards
    - Prices and monetary values
    - Pull quotes / blockquotes
  Everything else (nav, buttons, labels, body, captions, forms) = DM Sans.

RULE T2 — Weight discipline.
  DM Sans: 300 (body), 400 (ui default), 500 (emphasis, buttons)
  Cormorant: 300 (large display), 400 (normal), 500 (bold product names), italic (decorative)
  NEVER use weight 600 or 700 — too heavy against the warm palette.

RULE T3 — Sentence case everywhere.
  ✓ "Xem bộ sưu tập"
  ✗ "Xem Bộ Sưu Tập"  ← Title Case is wrong
  ✗ "XEM BỘ SƯU TẬP"  ← ALL CAPS is wrong (except 2-letter country codes, acronyms)
  Exception: brand name "VIETDUY" in logo lockup only.

RULE T4 — Line height.
  Display text (Cormorant): line-height 1.05–1.15
  Body copy (DM Sans):      line-height 1.65–1.75
  UI labels:                line-height 1.4

RULE T5 — Letter spacing.
  ALL CAPS labels/tags: letter-spacing 0.18–0.22em (even though we avoid ALL CAPS in copy)
  Display headings:     letter-spacing -0.01em (tighten slightly for large sizes)
  Body:                 letter-spacing 0 (never force-space body text)

RULE T6 — Max line length.
  Body paragraphs: max 68ch
  Captions: max 55ch
  Never allow full-width paragraphs on desktop.
```

### 2.4 Type Anti-patterns

```
✗ Mixing serif AND italic AND bold in the same sentence
✗ Font-size below 11px anywhere
✗ Placeholder text as label (always use <label> elements)
✗ Justified text alignment (always left or center)
✗ Underline on non-link text
✗ Using H tags for visual size only — maintain semantic hierarchy
```

---

## 3. SPATIAL SYSTEM (Spacing)

### 3.1 Spacing Scale

```css
/* Base unit: 4px */
--sp-1: 4px;
--sp-2: 8px;
--sp-3: 12px;
--sp-4: 16px;
--sp-5: 24px;
--sp-6: 32px;
--sp-7: 48px;
--sp-8: 64px;
--sp-9: 96px;
--sp-10: 128px;
--sp-11: 160px;
```

### 3.2 Spatial Rules

```
RULE S1 — Sections breathe.
  Vertical padding between major sections: min 96px desktop, 64px tablet, 48px mobile.
  Never compress section padding to make content fit — remove content instead.

RULE S2 — Cards have internal rhythm.
  Card padding: 20px sides, 20–24px top/bottom (always equal or more vertical than horizontal).
  Card gap in grid: 16–20px. Never less than 12px.

RULE S3 — The 8px grid.
  All spacing values must be multiples of 4px.
  Prefer multiples of 8px for major layout decisions.
  Never use arbitrary values like 13px, 17px, 22px for layout.

RULE S4 — Negative space is a design element.
  When a component looks crowded, double the padding before adding visual decoration.
  Empty space signals luxury — do not fill it.

RULE S5 — Max widths.
  Container max-width: 1440px
  Content column max-width: 1200px
  Article/prose max-width: 720px
  Always center with margin: 0 auto.
```

### 3.3 Layout Grid

```css
/* Main content layout — desktop */
.main-layout {
  display: grid;
  grid-template-columns: 3fr 1fr; /* Products 75% / Sidebar 25% */
  gap: var(--sp-8); /* 64px gap */
  max-width: 1440px;
  padding: 0 var(--sp-8); /* 64px side padding */
}

/* Product grid — inside left column */
.product-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: var(--sp-5); /* 24px */
}

/* Footer */
.footer-grid {
  display: grid;
  grid-template-columns: 1.4fr 1fr 1fr;
  gap: var(--sp-8);
}
```

---

## 4. MOTION SYSTEM

### 4.1 Timing Functions

```css
--ease-out: cubic-bezier(0.22, 1, 0.36, 1); /* Default — feels natural */
--ease-in-out: cubic-bezier(0.45, 0, 0.55, 1); /* Modals, overlays        */
--ease-spring: cubic-bezier(0.34, 1.56, 0.64, 1); /* Micro-interactions only */

--dur-instant: 100ms; /* State changes (active, checked)        */
--dur-fast: 180ms; /* Hover reveals, icon swaps              */
--dur-base: 280ms; /* Card lifts, color transitions          */
--dur-slow: 450ms; /* Page enters, drawer open               */
--dur-xslow: 700ms; /* Hero animations (one-time only)        */
```

### 4.2 Motion Rules

```
RULE M1 — Every animation must be purposeful.
  Ask: "Does this animation communicate something or just look pretty?"
  If "just look pretty" → remove it.
  Acceptable purposes: indicating state change, directing attention, providing feedback.

RULE M2 — No decorative page load animations.
  The hero text "flying in" from 40px below adds nothing.
  Fade-in opacity (0 → 1) over 400ms is acceptable for first paint only.
  Never animate transform on page load for elements below the fold.

RULE M3 — Hover is the primary animation trigger.
  Product cards: translateY(-6px) + box-shadow on hover — communicates interactivity.
  Images inside cards: scale(1.04) on parent hover — communicates zoomable detail.
  Nav links: underline width 0 → 100% — communicates active destination.

RULE M4 — Duration by distance.
  Small moves (4–8px): 180–280ms
  Medium moves (20–40px): 280–400ms
  Large moves (drawer, modal): 350–500ms
  Never animate a 4px shift for 700ms — feels sluggish.

RULE M5 — Respect prefers-reduced-motion.
  @media (prefers-reduced-motion: reduce) {
    *, *::before, *::after {
      animation-duration: 0.01ms !important;
      transition-duration: 0.01ms !important;
    }
  }
  This block must exist in every stylesheet.

RULE M6 — No infinite animations in production UI.
  Spinner/loader: acceptable while content loads (then destroy).
  Pulsing badge: never.
  Floating element: never.
```

### 4.3 Standard Transitions

```css
/* Apply these exact transitions — do not invent new ones */

/* Card lift */
.product-card {
  transition:
    transform var(--dur-base) var(--ease-out),
    box-shadow var(--dur-base) var(--ease-out),
    border-color var(--dur-base) var(--ease-out);
}
.product-card:hover {
  transform: translateY(-6px);
  box-shadow:
    0 16px 48px rgba(26, 24, 20, 0.13),
    0 4px 14px rgba(26, 24, 20, 0.06);
  border-color: var(--c-gold-lt);
}

/* Image zoom */
.product-card__img {
  transition: transform var(--dur-slow) var(--ease-out);
}
.product-card:hover .product-card__img {
  transform: scale(1.04);
}

/* Link underline */
.nav__link::after {
  transition: width var(--dur-base) var(--ease-out);
}

/* Button */
.btn {
  transition:
    background var(--dur-base) var(--ease-out),
    color var(--dur-base) var(--ease-out),
    border-color var(--dur-base) var(--ease-out),
    transform var(--dur-fast) var(--ease-out),
    box-shadow var(--dur-base) var(--ease-out);
}
.btn:hover {
  transform: translateY(-1px);
}
.btn:active {
  transform: translateY(0);
}
```

---

## 5. INTERACTION SYSTEM

### 5.1 Focus Styles (Accessibility)

```css
/* Global focus ring — NEVER remove outline without replacing it */
:focus-visible {
  outline: 2px solid var(--c-gold);
  outline-offset: 3px;
  border-radius: 2px;
}

/* Input focus */
.form__input:focus {
  border-color: var(--c-gold);
  box-shadow: 0 0 0 3px rgba(184, 150, 62, 0.18);
  outline: none;
}
```

### 5.2 Interactive States (every interactive element must have all 4)

```
State 1 — Default:   Base appearance
State 2 — Hover:     Subtle lift, color shift, cursor: pointer
State 3 — Focus:     Visible focus ring (keyboard users)
State 4 — Active:    Pressed feedback (scale down or darken)
State 5 — Disabled:  opacity: 0.4, cursor: not-allowed, no hover effect
```

### 5.3 Form Elements

```css
.form__input {
  font-family: var(--ff-body);
  font-size: var(--fs-base);
  font-weight: 300;
  color: var(--c-ink);
  background: var(--c-surface);
  border: 1px solid var(--c-border);
  border-radius: 2px;
  padding: 12px 16px;
  width: 100%;
  transition:
    border-color var(--dur-fast) var(--ease-out),
    box-shadow var(--dur-fast) var(--ease-out);
}
.form__input:hover {
  border-color: var(--c-border-s);
}
.form__input:focus {
  border-color: var(--c-gold);
  box-shadow: 0 0 0 3px rgba(184, 150, 62, 0.15);
  outline: none;
}
.form__input::placeholder {
  color: var(--c-ink-faint);
  font-weight: 300;
}
.form__input:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}
.form__input.is-invalid {
  border-color: var(--c-danger);
}
```

### 5.4 Button System

```css
/* Base */
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  font-family: var(--ff-body);
  font-size: var(--fs-xs);
  font-weight: 400;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  padding: 13px 40px;
  border-radius: 2px;
  cursor: pointer;
  white-space: nowrap;
  transition: all var(--dur-base) var(--ease-out);
}

/* Variants */
.btn--solid {
  background: var(--c-ink);
  color: var(--c-bg);
  border: 1px solid var(--c-ink);
}
.btn--outline {
  background: transparent;
  color: var(--c-ink);
  border: 1px solid var(--c-ink);
}
.btn--gold {
  background: var(--c-gold);
  color: #fff;
  border: 1px solid var(--c-gold);
}
.btn--ghost {
  background: transparent;
  color: var(--c-ink-mid);
  border: 1px solid var(--c-border-s);
}

/* Hover states */
.btn--solid:hover {
  background: #2a2520;
  border-color: #2a2520;
  transform: translateY(-1px);
  box-shadow: 0 4px 16px rgba(26, 24, 20, 0.15);
}
.btn--outline:hover {
  background: var(--c-ink);
  color: var(--c-bg);
}
.btn--gold:hover {
  background: var(--c-gold-dk);
  border-color: var(--c-gold-dk);
  transform: translateY(-1px);
}
.btn--ghost:hover {
  color: var(--c-ink);
  border-color: var(--c-ink);
}

/* Active */
.btn:active {
  transform: translateY(0);
  box-shadow: none;
}

/* Sizes */
.btn--sm {
  padding: 9px 24px;
  font-size: var(--fs-2xs);
}
.btn--lg {
  padding: 16px 56px;
  font-size: var(--fs-xs);
}

/* Icon button */
.btn-icon {
  width: 36px;
  height: 36px;
  padding: 0;
  display: grid;
  place-items: center;
  border: 1px solid var(--c-border);
  border-radius: 50%;
  color: var(--c-ink-lt);
  font-size: 14px;
  transition: all var(--dur-fast) var(--ease-out);
}
.btn-icon:hover {
  background: var(--c-ink);
  color: var(--c-bg);
  border-color: var(--c-ink);
}
```

---

## 6. COMPONENT LIBRARY

### 6.1 Navigation

```
Structure:
  [BRAND LOGO]    [Home · Sản phẩm · Liên hệ]    [Cart icon · CTA button]

  3-column CSS Grid: 200px | 1fr | 200px
  Height: 72px, sticky, blur backdrop

Rules:
- Brand name: Cormorant Garamond, 1.5rem, tracking 0.22em — display ONLY as "Vietduy"
- Nav links: DM Sans, 11px, tracking 0.18em, sentence case (not "Trang Chủ" but "Trang chủ")
- Active link: underline via ::after pseudo, width 100%, color --c-gold
- Background: rgba(247,245,242, 0.92) + backdrop-filter: blur(20px)
- Border-bottom: 1px solid var(--c-border)
- Mobile: hide nav links (hamburger), keep brand + cart + cta
```

### 6.2 Product Card

```html
<!-- CANONICAL product card structure -->
<a href="/san-pham/{slug}" class="product-card">
  <div class="product-card__media">
    <img src="{url}" alt="{name}" class="product-card__img" loading="lazy" />
    <!-- Badge: only if product.badge exists -->
    <span class="product-card__badge product-card__badge--{style}"
      >{badge}</span
    >
    <!-- Overlay with quick-view — appears on hover -->
    <div class="product-card__overlay">
      <button class="product-card__quick">Xem nhanh</button>
    </div>
  </div>
  <div class="product-card__body">
    <p class="product-card__cat">{category}</p>
    <h3 class="product-card__name">{name}</h3>
    <p class="product-card__desc">{shortDesc}</p>
    <div class="product-card__footer">
      <span class="product-card__price">${price}</span>
      <button class="btn-icon product-card__wish" aria-label="Yêu thích">
        <i class="fa-regular fa-heart"></i>
      </button>
    </div>
  </div>
</a>

Rules: - Aspect ratio of image: always 4:3 - Card border-radius: 4px (subtle,
not rounded) - Name: Cormorant Garamond, font-size var(--fs-display-sm) -
Category: DM Sans, 10px, tracking 0.16em, uppercase, color var(--c-ink-lt) -
Price: Cormorant Garamond, font-size var(--fs-lg), weight 500 - Description: 2
lines max, -webkit-line-clamp: 2 - Hover: card translateY(-6px) + image
scale(1.04) + show overlay button
```

### 6.3 Product Detail Page

```
Layout (desktop):
  [←←← 55% IMAGE GALLERY ←←←] [→→→ 45% INFO PANEL →→→]
  Position: sticky info panel, gallery scrolls

Image Gallery:
  - Main image: aspect-ratio 3/4
  - Thumbnail strip: horizontal scroll, 4 thumbs visible, 80×80px each
  - Active thumb: 2px solid var(--c-gold) border

Info Panel structure (top → bottom):
  1. Breadcrumb: "Trang chủ / Túi xách / Riviera Shoulder Bag"  [font-size 11px]
  2. Category tag                                                  [font-size 10px, uppercase]
  3. Product name (H1)                                            [Cormorant, display-lg]
  4. Star rating + review count                                   [gold stars, 13px text]
  5. Price (primary) + Original price strikethrough (if sale)     [Cormorant, 2rem]
  6. Short description                                            [DM Sans, 15px, max 3 lines]
  7. Color selector (visual swatches)
  8. Size selector (text buttons)
  9. Quantity stepper                                             [−][1][+]
  10. Add to cart button (full width, btn--solid)
  11. Wishlist link (ghost, smaller)
  12. Divider
  13. Materials, Dimensions, Shipping (accordion or stacked)
  14. Back link → "← Tiếp tục mua sắm"
```

### 6.4 Sidebar

```
Desktop: sticky, top: calc(72px + 24px) [nav height + gap]
Mobile: static, full width, appears below product list

Widgets (top to bottom):
  1. About widget    — brand story excerpt + link
  2. Divider         — 1px solid var(--c-border), full width
  3. Services list   — 4 items with FontAwesome icons
  4. Divider
  5. Newsletter form — email input + submit button inline
  6. Divider
  7. Social links    — Facebook, Instagram, X, Pinterest (4 links)

Social link anatomy:
  [icon 13px] [label "Instagram"] [arrow-up-right — appears on hover]
  Arrow: opacity 0 + translateX(-4px) → opacity 1 + translateX(0) on hover
```

### 6.5 Hero Section (Homepage)

```
Layout: 2 columns, 50/50 split
Left:  text content, left-aligned
Right: editorial photo, full bleed with gradient overlay

Left column:
  - Eyebrow tag:   "Collection SS 2025" — 10px, tracking 0.25em, color --c-gold
  - H1:            Cormorant Garamond, clamp(3.5rem, 7vw, 6rem), weight 300
  - Subheadline:   DM Sans, 17px, color --c-ink-mid, max 2 lines
  - CTA buttons:   btn--solid + btn--outline, side by side, gap 16px
  - Stats row:     3 numbers (year, experience, clients) — Cormorant 2rem + DM Sans 11px label

Right column:
  - Image: object-fit cover, full height
  - Overlay: gradient rgba(26,24,20, 0.0) → rgba(26,24,20, 0.4) from right edge inward
  - Scroll indicator: bottom-left corner, vertical text + animated line

Animation (ONE-TIME on page load only):
  - Left content: opacity 0 → 1, translateY(24px) → 0, duration 600ms, ease-out
  - Right image:  opacity 0 → 1, duration 800ms, ease-out, delay 200ms
  - No other page-load animations anywhere on the page
```

### 6.6 Cart Sidebar (Drawer)

```
Structure: off-canvas drawer from right edge
Width: 420px desktop, 100vw mobile
Backdrop: rgba(26,24,20, 0.6) scrim behind drawer
Open animation: translateX(100%) → translateX(0), 350ms ease-out
Close animation: translateX(0) → translateX(100%), 280ms ease-in-out

Sections (top to bottom):
  Header: "Giỏ hàng" + close button (X)
  Body:   scrollable list of cart items
    Each item: [thumbnail 64×64] [name + variant] [qty stepper] [price] [remove]
  Footer (sticky):
    Coupon input row
    Subtotal, Shipping fee, Total breakdown
    "Thanh toán" button (btn--solid, full width)
    "Tiếp tục mua sắm" link (centered, smaller)
```

### 6.7 Filter System (Products Page) — Breakthrough Feature

```
This is the "breakthrough" filter UX. Render as a floating horizontal pill bar,
sticky below the nav, not as a left sidebar.

Structure:
  [All] [Túi xách] [Đồng hồ] [Lụa & Cashmere] [Nước hoa] [Thời trang] [Trang sức]
  [▼ Giá: $0 – $25,000] [▼ Sắp xếp: Mặc định] [⊞ Grid] [☰ List]

Pill styles:
  Default: bg transparent, border 1px solid --c-border, color --c-ink-mid
  Active:  bg --c-ink, border --c-ink, color --c-bg
  Hover:   border-color --c-border-s, color --c-ink

Price range: custom dual-handle slider, track color --c-gold
Sort dropdown: appear below trigger, 200ms fade+slide

View toggle (Grid/List):
  Grid: default, 2-3 col product grid
  List: single column, image left + details right, more info visible

Filter bar sticky behavior:
  Sticks at top: 72px (nav height)
  Has its own blur backdrop when sticky
  On mobile: horizontal scroll (no wrap)
```

### 6.8 Footer

```
Background: var(--c-bg-dark) (#1A1814)
Layout: 3-column CSS Grid (desktop) → 1-column (mobile)
Columns: [Brand + Address + Social] [Navigation links] [Contact info]

Rules:
- Brand name: Cormorant Garamond, 1.5rem, tracking 0.22em, color var(--c-bg)
- Tagline: Cormorant italic, color var(--c-gold-lt)
- Headings: 10px, uppercase, tracking 0.20em, color var(--c-gold)
- Links: 13px, weight 300, color rgba(247,245,242, 0.5), hover → rgba(247,245,242, 1.0)
- Social icons: 32×32px circles, border 1px solid rgba(255,255,255,0.15),
                hover → bg rgba(255,255,255,0.1)
- Bottom bar: copyright + legal links, color rgba(247,245,242, 0.28)
- Margin-top on footer: 120px (visual breathing room from last section)
```

---

## 7. RESPONSIVE SYSTEM

### 7.1 Breakpoints

```css
/* Use these ONLY — no arbitrary breakpoints */
--bp-sm: 480px; /* Mobile large (landscape phone)        */
--bp-md: 768px; /* Tablet portrait — MAJOR layout switch */
--bp-lg: 1024px; /* Tablet landscape / small desktop      */
--bp-xl: 1280px; /* Standard desktop                      */
--bp-2xl: 1440px; /* Large desktop (max-width container)   */
```

```css
/* Media query usage */
@media (max-width: 768px) {
  /* Mobile-specific overrides   */
}
@media (max-width: 1024px) {
  /* Tablet-specific overrides   */
}
@media (min-width: 1440px) {
  /* Large desktop enhancements  */
}
```

### 7.2 Layout at Each Breakpoint

```
Desktop (>1024px):
  - Main layout: 3fr 1fr (products + sidebar)
  - Product grid: 2–3 columns (auto-fill, minmax 260px)
  - Nav: 3-column (brand | links | actions)
  - Footer: 3-column grid
  - Hero: 2-column split

Tablet (768–1024px):
  - Main layout: 1fr (sidebar moves below or hides)
  - Product grid: 2 columns
  - Nav: brand + hamburger only
  - Footer: 2-column or 1-column

Mobile (<768px):
  - Everything: 1 column
  - Product grid: 1 column
  - Hero: single column, image below text
  - Cart drawer: full width (100vw)
  - Filter bar: horizontal scroll, no wrap
  - Font sizes: use clamp() values, never hard code small sizes
```

### 7.3 Responsive Rules

```
RULE R1 — Mobile content priority.
  On mobile, order: hero → featured products → categories → footer.
  Sidebar content is deprioritized and collapsed.

RULE R2 — Touch targets.
  All interactive elements: minimum 44×44px on mobile.
  Spacing between tappable items: minimum 8px.

RULE R3 — Image sizing.
  Always use loading="lazy" except above-the-fold hero.
  Always set explicit width and height to prevent CLS.
  Always use object-fit: cover with defined aspect-ratio.

RULE R4 — No horizontal overflow.
  overflow-x: hidden on body.
  Test every component at 320px width.
```

---

## 8. UX WRITING GUIDELINES

### 8.1 Vietnamese Content Rules

```
Casing:
  ✓ "Xem bộ sưu tập"          ← sentence case
  ✗ "Xem Bộ Sưu Tập"          ← title case
  ✗ "XEM BỘ SƯU TẬP"          ← all caps (except 2-letter codes, brand name in logo)

CTA verbs — use action-first, specific:
  ✓ "Xem chi tiết"    ✓ "Thêm vào giỏ"   ✓ "Tiếp tục mua sắm"
  ✓ "Khám phá"        ✓ "Đặt hàng ngay"  ✓ "Liên hệ tư vấn"
  ✗ "Click here"      ✗ "Submit"         ✗ "OK"    ✗ "Yes/No"

Error messages — human, not technical:
  ✓ "Vui lòng nhập địa chỉ email hợp lệ."
  ✗ "Invalid email format."
  ✗ "Error 422: Validation failed."

Empty states — helpful, not apologetic:
  ✓ "Chưa có sản phẩm trong danh mục này. Khám phá bộ sưu tập khác?"
  ✗ "No results found."
  ✗ "Sorry, nothing here."

Price formatting:
  ✓ $2,450.00    (always 2 decimal places, comma thousands separator)
  ✗ $2450        ✗ 2.450$      ✗ USD 2,450
```

### 8.2 Micro-copy Standards

```
Loading state   : "Đang tải..."         (not "Loading..." or spinner-only)
Success toast   : "Đã thêm vào giỏ hàng ✓"
Error toast     : "Có lỗi xảy ra. Vui lòng thử lại."
Cart empty      : "Giỏ hàng của bạn đang trống."
No products     : "Không tìm thấy sản phẩm phù hợp."
Out of stock    : "Tạm hết hàng"
Order confirmed : "Đơn hàng đã được xác nhận"
Back button     : "← Tiếp tục mua sắm"
Required field  : Asterisk (*) with legend "Thông tin bắt buộc"
```

---

## 9. ACCESSIBILITY BASELINE

```
Contrast minimums (WCAG AA):
  Normal text (<18px): 4.5:1 ratio
  Large text (≥18px bold or ≥24px): 3:1 ratio
  UI components (buttons, inputs): 3:1 ratio

  --c-gold (#B8963E) on --c-bg (#F7F5F2): 4.6:1 ✓
  --c-ink (#1A1814) on --c-bg (#F7F5F2):  16.2:1 ✓
  --c-ink-mid on --c-bg:                  7.8:1 ✓
  --c-ink-lt on --c-bg:                   3.9:1 ✓ (large text only)

Required on every page:
  ✓ <html lang="vi">
  ✓ All images: meaningful alt text (or alt="" for decorative)
  ✓ All icon-only buttons: aria-label
  ✓ All forms: <label> associated with input (not placeholder-as-label)
  ✓ Skip-to-content link as first element: <a href="#main" class="skip-link">
  ✓ Focus visible on all interactive elements
  ✓ ARIA roles: role="navigation", role="main", role="complementary", role="contentinfo"
  ✓ prefers-reduced-motion media query (see Rule M5)
```

---

## 10. ANTI-PATTERNS — NEVER DO THESE

```
VISUAL
  ✗ Gradient text (background-clip: text) — brand killer
  ✗ Box shadow with color tint (gold/amber glow) — looks cheap
  ✗ Nested cards (card inside card inside card)
  ✗ More than 1 accent per component
  ✗ Border-radius > 8px on product cards (should feel architectural, not bubbly)
  ✗ Thin hairline text on colored backgrounds (trust contrast checker)
  ✗ Decorative separators (wavy lines, ornamental dividers) — use plain 1px solid

TYPOGRAPHY
  ✗ Three or more font weights in one component
  ✗ Letter-spacing on body text
  ✗ Italic Cormorant + Bold DM Sans on same line (clashes)
  ✗ Font-size: 10px for any user-facing content
  ✗ ALL CAPS for sentences > 3 words

LAYOUT
  ✗ Centered body text beyond 2 lines
  ✗ Full-width paragraphs on desktop (max 68ch)
  ✗ Horizontal scrollbar on page (overflow-x: hidden)
  ✗ Empty state with just "No data" — always add guidance

INTERACTION
  ✗ Hover effects that hide content (content must be accessible without hover on mobile)
  ✗ Links that look like buttons or buttons that look like links (pick one)
  ✗ Form submit with no loading feedback
  ✗ No error state on forms (always show inline validation)
  ✗ Toast that disappears in under 3 seconds

PERFORMANCE
  ✗ Images without explicit width/height attributes (causes CLS)
  ✗ No loading="lazy" on below-fold images
  ✗ @import for Google Fonts (use <link> instead)
  ✗ JavaScript that blocks render for non-critical features

COPY
  ✗ Placeholder text as labels
  ✗ Ellipsis on buttons ("Add to cart...")
  ✗ Double punctuation
  ✗ "Click here" or "Learn more" as link text (not descriptive)
```

---

## 11. RAZOR VIEWS — ASP.NET CORE SPECIFIC

```csharp
// ── Layout file ──
// Use consistent section names across all pages:
@RenderSection("Styles",  required: false)    // Page-specific CSS
@RenderSection("Scripts", required: false)    // Page-specific JS (before </body>)

// ── Image rendering ──
// Always use null-coalescing for image URLs:
<img src="@(Model.ThumbnailUrl ?? "/images/placeholder-product.webp")"
     alt="@Model.Name"
     width="600" height="450"
     loading="lazy"
     class="product-card__img" />

// ── Price formatting ──
// Use a consistent display helper:
@Model.Price.ToString("C", new System.Globalization.CultureInfo("en-US"))
// Output: $2,450.00

// ── CSS class conditionals ──
<a class="nav__link @(ViewContext.RouteData.Values["controller"]?.ToString() == "Home" ? "nav__link--active" : "")">
  Trang chủ
</a>

// ── Partial views for components ──
// Each component has its own _partial:
@await Html.PartialAsync("_ProductCard", product)
@await Html.PartialAsync("_CartSidebar")
@await Html.PartialAsync("_FilterBar", Model.FilterState)
@await Html.PartialAsync("_Pagination", Model.PageInfo)

// ── Empty state ──
@if (!Model.Products.Any())
{
  <div class="empty-state">
    <i class="fa-regular fa-face-thinking empty-state__icon"></i>
    <p class="empty-state__text">Không tìm thấy sản phẩm phù hợp.</p>
    <a href="/san-pham" class="btn btn--outline">Xem tất cả sản phẩm</a>
  </div>
}

// ── Validation styling ──
// Pair ASP.NET validation with CSS classes:
<div class="form__group @(ViewData.ModelState["Email"]?.Errors.Count > 0 ? "has-error" : "")">
  <label class="form__label" asp-for="Email">Email <span aria-hidden="true">*</span></label>
  <input class="form__input" asp-for="Email" autocomplete="email" />
  <span class="form__error" asp-validation-for="Email"></span>
</div>
```

---

## 12. FILE & CLASS NAMING

```
CSS classes: BEM methodology
  Block:     .product-card
  Element:   .product-card__img, .product-card__name, .product-card__price
  Modifier:  .product-card--featured, .btn--solid, .nav__link--active

CSS files (one per page/component):
  _variables.css      ← all CSS custom properties
  _reset.css          ← normalize + base
  _typography.css     ← type scale + utilities
  _layout.css         ← grid, container, main-layout
  _nav.css            ← header + navigation
  _hero.css           ← homepage hero
  _product-card.css   ← shared card component
  _product-grid.css   ← grid layout for cards
  _product-detail.css ← detail page
  _sidebar.css        ← sidebar + widgets
  _cart.css           ← cart drawer
  _filter.css         ← filter pill bar
  _forms.css          ← all form elements
  _buttons.css        ← button system
  _footer.css         ← footer
  _admin.css          ← admin area overrides
  main.css            ← imports all above in order

Razor partials: _PascalCase.cshtml
  _ProductCard.cshtml
  _FilterBar.cshtml
  _CartSidebar.cshtml
  _Pagination.cshtml
  _EmptyState.cshtml
  _Breadcrumb.cshtml
  _StarRating.cshtml
  _SocialLinks.cshtml
```

---

## 13. QUICK REFERENCE CARD

```
┌─────────────────────────────────────────────────────────────────────┐
│  VIETDUY DESIGN QUICK REFERENCE                                     │
├─────────────┬───────────────────────────────────────────────────────┤
│ Font display│ Cormorant Garamond — H1/H2/product name/price only    │
│ Font UI     │ DM Sans — everything else                             │
├─────────────┼───────────────────────────────────────────────────────┤
│ Primary BG  │ #F7F5F2  Warm parchment                               │
│ Primary ink │ #1A1814  Near-black warm                              │
│ Accent      │ #B8963E  Champagne gold (4.6:1 contrast ✓)            │
│ Border      │ #E4DED6  Warm light grey                              │
├─────────────┼───────────────────────────────────────────────────────┤
│ Ease        │ cubic-bezier(0.22, 1, 0.36, 1)                        │
│ Duration    │ 180ms fast · 280ms base · 450ms slow                  │
│ Card hover  │ translateY(-6px) + shadow + border-color              │
├─────────────┼───────────────────────────────────────────────────────┤
│ Spacing     │ 4px base unit · always multiples of 4                 │
│ Section gap │ 96px desktop · 64px tablet · 48px mobile              │
│ Max-width   │ 1440px container · 720px prose                        │
├─────────────┼───────────────────────────────────────────────────────┤
│ Casing      │ Sentence case always · no Title Case · no ALL CAPS    │
│ Price       │ $2,450.00 format always                               │
│ Back button │ "← Tiếp tục mua sắm"                                  │
├─────────────┼───────────────────────────────────────────────────────┤
│ NEVER       │ Gradient text · color glow shadows · nested cards     │
│ NEVER       │ Font < 11px · 3+ font weights per component           │
│ NEVER       │ Remove :focus-visible without replacing it            │
│ NEVER       │ Images without alt text · buttons without label       │
└─────────────┴───────────────────────────────────────────────────────┘
```

---

_VIETDUY DESIGN SKILL v1.0 — Load this file before every UI task._  
_When in doubt: subtract, don't add. Luxury is restraint._
