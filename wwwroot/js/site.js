// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var $skillbox = $('.skillbox:first');
var $skillbars = $('.skillbar');
var $window = $(window);
var animated = false;

function skillbarAnimation() {
    if (animated) {
        return;
    }
    
    animated = true;
    
    $skillbars.each(function(){
        $(this).find('.bar').each(function( index ) {
            
            var animationSpeed = (Math.floor(Math.random() * 1000 ) + 500);
            $(this).css("width", "0px");
            $(this).animate({
                width:$(this).data("originalWidth")
            },animationSpeed);
        });
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
    $('.skillbar').each(function(){
        $(this).find('.bar').each(function( index ) {
            $(this).data("originalWidth", $(this).css("width"));
            $(this).css("width", "0px");
        });
    });

    $window.on('scroll resize', check_if_in_view);
    $window.trigger('scroll');
});