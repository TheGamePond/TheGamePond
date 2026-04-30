document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll("[data-product-gallery]").forEach((gallery) => {
    const mainImage = gallery.querySelector("[data-product-gallery-main]");
    const thumbs = gallery.querySelectorAll("[data-product-gallery-thumb]");

    if (!mainImage || thumbs.length === 0) {
      return;
    }

    thumbs.forEach((thumb) => {
      thumb.addEventListener("click", () => {
        const imageSrc = thumb.getAttribute("data-image-src");
        const imageAlt = thumb.getAttribute("data-image-alt") || "";

        if (!imageSrc) {
          return;
        }

        mainImage.setAttribute("src", imageSrc);
        mainImage.setAttribute("alt", imageAlt);

        thumbs.forEach((item) => {
          item.classList.remove("active");
          item.setAttribute("aria-pressed", "false");
        });

        thumb.classList.add("active");
        thumb.setAttribute("aria-pressed", "true");
      });
    });
  });
});
