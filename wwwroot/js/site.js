// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var $skillbox = $('.skill-container:first');
var $skillboxes = $('.skill-box');
var $window = $(window);
var animated = false;

function skillbarAnimation() {
    if (animated) {
        return;
    }
    animated = true;
    $skillboxes.each(function(){
        var animationSpeed = (Math.floor(Math.random() * 1000 ) + 500);
        var originalWidthBar = $(this).data("originalWidthBar");
        var originalWidthDot = $(this).data("originalWidthDot");
        $(this).find('.bar').each(function( index, value, parent = $(this) ) {
            $(this).animate({
                width:originalWidthBar
            },animationSpeed);
        });
        $(this).find('.skill-dot-bar').each(function( index, value, parent = $(this) ) {
            $(this).animate({
                width:originalWidthDot
            },animationSpeed);
        })
    });
}

function check_if_in_view() {
    var window_height = $window.height();
    var window_top_position = $window.scrollTop();
    var window_bottom_position = (window_top_position + window_height);
    
    var skillbox_height = $skillbox.outerHeight();
    var skillbox_top_position = $skillbox.offset().top;
    var skillbox_bottom_position = (skillbox_top_position + skillbox_height);

    if ((skillbox_bottom_position >= window_top_position) &&
        (skillbox_top_position <= window_bottom_position)) {
        skillbarAnimation();
    }
}

$(document).ready(function(){
    $skillboxes.each(function(){
        var originalWidthBar;
        var originalWidthDot;
        $(this).find('.bar').each(function( index, value, parent = $(this) ) {
            //parent.data("originalWidth", $(this).css("width"));
            originalWidthBar = $(this).css("width");
            $(this).css("width", "15px");
        });
        $(this).find('.skill-dot-bar').each(function( index, value, parent = $(this) ) {
            originalWidthDot = $(this).css("width");
            $(this).css("width", "30px");
        });
        $(this).data("originalWidthBar", originalWidthBar);
        $(this).data("originalWidthDot", originalWidthDot);
    });

    $window.on('scroll resize', check_if_in_view);
    $window.trigger('scroll');
});