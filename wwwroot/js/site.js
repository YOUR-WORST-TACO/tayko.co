// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let $skillbox = $('.skill-container:first');
let $skillboxes = $('.skill-box');
let $window = $(window);
let animated = false;%

function skillbarAnimation() {
    if (animated) {
        return;
    }
    animated = true;
    $skillboxes.each(function(){
        let animationSpeed = (Math.floor(Math.random() * 1000 ) + 500);
        let originalWidthBar = $(this).data("originalWidthBar");
        let originalWidthDot = $(this).data("originalWidthDot");
        $(this).find('.bar').each(function() {
            $(this).animate({
                width:originalWidthBar
            },animationSpeed);
        });
        $(this).find('.skill-dot-bar').each(function() {
            $(this).animate({
                width:originalWidthDot
            },animationSpeed);
        })
    });
}

function check_if_in_view() {
    let window_height = $window.height();
    let window_top_position = $window.scrollTop();
    let window_bottom_position = (window_top_position + window_height);
    
    let skillbox_height = $skillbox.outerHeight();
    let skillbox_top_position = $skillbox.offset().top;
    let skillbox_bottom_position = (skillbox_top_position + skillbox_height);

    if ((skillbox_bottom_position >= window_top_position) &&
        (skillbox_top_position <= window_bottom_position)) {
        skillbarAnimation();
    }
}

$(document).ready(function(){
    $skillboxes.each(function(){
        let originalWidthBar = 0;
        let originalWidthDot = 0;
        $(this).find('.bar').each(function() {
            //parent.data("originalWidth", $(this).css("width"));
            originalWidthBar = $(this).css("width");
            $(this).css("width", "15px");
        });
        $(this).find('.skill-dot-bar').each(function() {
            originalWidthDot = $(this).css("width");
            $(this).css("width", "30px");
        });
        $(this).data("originalWidthBar", originalWidthBar);
        $(this).data("originalWidthDot", originalWidthDot);
    });

    $window.on('scroll resize', check_if_in_view);
    $window.trigger('scroll');
});