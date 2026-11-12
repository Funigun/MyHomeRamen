# Design System Specification

## 1. Overview & Creative North Star: "The Hearthside Manuscript"
This design system rejects the clinical, "app-like" feel of standard digital interfaces in favor of a high-end editorial experience. Our Creative North Star is **The Hearthside Manuscript**: a digital translation of a hand-bound culinary journal. 

The goal is to evoke the tactile warmth of a Kyoto ramen shop—weathered wood, steam, and artisanal craft—through sophisticated layout choices. We achieve this by breaking the traditional grid with intentional asymmetry, overlapping high-serif typography over soft tonal backgrounds, and utilizing "breathing room" as a luxury element. Every screen should feel like a curated page from a premium publication, not a collection of modular boxes.

## 2. Colors & Tonal Depth
The palette is rooted in organic, earth-derived tones. The primary colors simulate aged cedar and rich broth, while the surfaces mimic the off-white texture of washi paper.

*   **Primary Roles:** Use `primary` (#6c2f00) for high-impact brand moments and `primary_container` (#8b4513) for depth. Use `tertiary` (#45422e) sparingly for functional grounding.
*   **The "No-Line" Rule:** Standard 1px solid borders are strictly prohibited for sectioning. Definition must be achieved through background shifts. For example, a "Daily Specials" section should use `surface_container_low` set against a `surface` background. The eye should perceive change through tonal transition, not structural lines.
*   **Surface Hierarchy & Nesting:** Treat the UI as physical layers. 
    *   Base: `surface` (#fff8f6)
    *   Secondary Content: `surface_container` (#ffe9e5)
    *   Elevated Cards: `surface_container_highest` (#ffdad4)
*   **The "Glass & Gradient" Rule:** To provide "soul" to the UI, use subtle radial gradients on hero backgrounds transitioning from `primary` to `primary_container`. For floating navigation bars or overlay menus, use **Glassmorphism**: a background of `surface_variant` at 80% opacity with a `20px` backdrop blur.
*   **Signature Textures:** Apply a subtle grain overlay or a `surface_tint` wash to large background areas to avoid the "flatness" of digital HEX codes.

## 3. Typography: The Editorial Voice
We use a high-contrast typographic scale to create an authoritative, premium rhythm.

*   **Display & Headlines (`newsreader`):** This serif font is our "signature." Use `display-lg` (3.5rem) and `headline-lg` (2rem) to create editorial "hooks." These should often be center-aligned or placed with intentional asymmetry to draw the eye.
*   **Title & Body (`beVietnamPro`):** This sans-serif provides modern legibility. Use `title-md` (1.125rem) for menu categories and `body-lg` (1rem) for descriptions. 
*   **The Hierarchy Strategy:** A large `display-md` headline should often be paired with a much smaller `label-md` (all-caps) sub-header to create a "Big/Small" dynamic found in high-fashion magazines.

## 4. Elevation & Depth
In this design system, depth is "baked into" the color tokens rather than added with harsh shadows.

*   **The Layering Principle:** Place `surface_container_lowest` (#ffffff) elements (like a featured ramen card) onto a `surface_container_low` (#fff0ee) background. This creates a soft, natural "lift."
*   **Ambient Shadows:** If a floating action is required, shadows must be `on_surface` (#2b1613) at 6% opacity with a `32px` blur and a `12px` Y-offset. This mimics the soft, diffused light of a paper lantern.
*   **The "Ghost Border" Fallback:** If accessibility requires a container boundary, use the `outline_variant` (#dac2b6) at **15% opacity**. It should be felt, not seen.
*   **Glassmorphism:** Use `surface_bright` with a 70% alpha for overlays, creating a "frosted glass" effect that allows the warm wood tones of the background to bleed through.

## 5. Components

*   **Buttons:** 
    *   *Primary:* Use a linear gradient (`primary` to `primary_container`) with `on_primary` text. Corners use `md` (0.375rem) roundedness—avoid "pill" shapes as they feel too casual.
    *   *Secondary:* Use `secondary_container` with no border. 
*   **Chips:** Use `tertiary_fixed_dim` for category chips (e.g., "Spicy," "Vegetarian"). Use `sm` (0.125rem) rounding for a more architectural, rustic look.
*   **Input Fields:** Use a "Minimalist Ledger" style. Only a bottom stroke using `outline` (#877369). When focused, the stroke transitions to `primary` (#6c2f00) and the background shifts to `surface_container_low`.
*   **Cards:** Forbid divider lines. Separate "Ingredients" from "Price" using `spacing-6` (2rem) of vertical white space or a subtle background shift to `surface_container_high`.
*   **Custom Component: The "Chef’s Highlight":** An asymmetrical card using `surface_container_highest` with a `display-sm` serif title overlapping the edge of the container by `spacing-2`.
*   **Custom Component: Seasonal Badge:** A vertical "Label" component using `on_secondary_fixed_variant` text, rotated 90 degrees, placed on the far right of menu items to mimic Japanese vertical calligraphy.

## 6. Do's and Don'ts

### Do:
*   **Embrace Negative Space:** Use `spacing-16` or `spacing-20` between major sections to let the high-end typography breathe.
*   **Layer Surfaces:** Think of the UI as "Sheets of Paper." A card sits on a section, which sits on the page background.
*   **Use Tonal Transitions:** Define the end of a page with a full-bleed shift from `surface` to `surface_container_lowest`.

### Don't:
*   **Never use 100% Black:** Always use `on_background` (#2b1613) or `on_surface` (#2b1613) for text to maintain the "Dark Chocolate" warmth.
*   **Avoid "Floating" Buttons:** Unless it’s a critical Action Button, anchor elements to the grid or surface layers. 
*   **No Geometric Perfection:** Avoid "full" (9999px) roundedness on large containers. The restaurant is rustic; use `lg` (0.5rem) or `xl` (0.75rem) to maintain a hand-crafted feel.
*   **No Divider Lines:** If you feel the need to add a line, add `1.4rem` (spacing-4) of empty space instead.