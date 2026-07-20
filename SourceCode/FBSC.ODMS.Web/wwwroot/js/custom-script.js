/* --- Custom JS Should Be Here --- */

$(document).ready(function () {

    // Responsive sidebar: start collapsed (icon rail) below the tablet
    // breakpoint so it doesn't eat most of a narrow viewport, expanded on
    // desktop as before. Reuses the existing #sidebarToggleTop/body.sidebar-toggled
    // mechanism from site.js - manual toggling still works at any width;
    // this only re-applies the "natural" state when a resize actually
    // crosses the breakpoint, so it never fights a deliberate manual choice
    // made without resizing.
    var sidebarBreakpoint = 992;
    var body = document.body;

    function sidebarBucket() {
        return (window.innerWidth || document.documentElement.clientWidth) < sidebarBreakpoint ? 'small' : 'large';
    }

    var lastSidebarBucket = sidebarBucket();

    if (lastSidebarBucket === 'small') {
        body.classList.add('sidebar-toggled');
    }

    var sidebarResizeTimer;
    window.addEventListener('resize', function () {
        clearTimeout(sidebarResizeTimer);
        sidebarResizeTimer = setTimeout(function () {
            var bucket = sidebarBucket();
            if (bucket !== lastSidebarBucket) {
                lastSidebarBucket = bucket;
                body.classList.toggle('sidebar-toggled', bucket === 'small');
            }
        }, 150);
    });

});
