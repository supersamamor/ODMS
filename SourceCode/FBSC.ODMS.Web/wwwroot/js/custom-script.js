/* --- Custom JS Should Be Here --- */

$(document).ready(function () {

    // Responsive sidebar: collapsed (icon rail) below the tablet breakpoint,
    // expanded on desktop. Reuses the existing #sidebarToggleTop/
    // body.sidebar-toggled mechanism from site.js. matchMedia fires only on
    // real breakpoint crossings, so a manual toggle holds until the viewport
    // actually crosses 992px, at which point the natural state for the new
    // size is re-applied.
    var sidebarMediaQuery = window.matchMedia('(max-width: 991.98px)');

    function applySidebarState(mq) {
        document.body.classList.toggle('sidebar-toggled', mq.matches);
    }

    applySidebarState(sidebarMediaQuery);

    if (typeof sidebarMediaQuery.addEventListener === 'function') {
        sidebarMediaQuery.addEventListener('change', applySidebarState);
    } else if (typeof sidebarMediaQuery.addListener === 'function') {
        // Older Safari/Edge fallback
        sidebarMediaQuery.addListener(applySidebarState);
    }

});
