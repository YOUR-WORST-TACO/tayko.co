// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/*let $skillbox = $('.skill-container:first');
let $skillboxes = $('.skill-box');
let $window = $(window);
let animated = false;

function skillbarAnimation() {
    if (animated) {
        return;
    }
    animated = true;
    $skillboxes.each(function(){
        let animationSpeed = (Math.floor(Math.random() * 1000 ) + 500);
        let originalWidthBar = $(this).data("originalWidthBar");
        
        $(this).find('.bar').each(function() {
            $(this).animate({
                width:originalWidthBar
            },animationSpeed);
        });
        $(this).find('.skill-dot-bar').each(function() {
            $(this).animate({
                width:(parseInt(originalWidthBar)+10).toString() + "px"
            },animationSpeed);
        })
    });
}

function skillbarReset() {
    if (!animated) {
        return;
    }
    
    animated = false;

    $skillboxes.each(function(){
        $(this).find('.bar').each(function() {
            $(this).stop();
            $(this).css("width", "10px");
        });
        $(this).find('.skill-dot-bar').each(function() {
            $(this).stop();
            $(this).css("width", "20px");
        });
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
    } else {
        skillbarReset();
    }
}

$(document).ready(function(){
    $skillboxes.each(function(){
        let originalWidthBar = 0;
        $(this).find('.bar').each(function() {
            originalWidthBar = $(this).css("width");
            $(this).css("width", "10px");
        });
        $(this).find('.skill-dot-bar').each(function() {
            $(this).css("width", "20px");
        });
        $(this).data("originalWidthBar", originalWidthBar);
    });

    $window.on('scroll resize', check_if_in_view);
    $window.trigger('scroll');
});*/



//$('#title').html("Who is &lt;author>");

function swapText(element = null, startText = null, endText = "", prefix = "", postfix = "", oncomplete = null) {
    
    if (element == null) {
        throw "No swap text element provided!";
    }
    
    if (startText == null) {
        startText = element.html();
    }
    
    let steps = [];
    for (let i = 0; i < startText.length; i++) {
        steps.push(startText.substring(0, startText.length - i))
    }
    
    for (let i = 0; i <= endText.length; i++) {
        steps.push(endText.substring(0, i))
    }
    
    let callBack = function(fn, elm, next, pre, post, callback) {
        let newContent = pre + next[0] + post;
        elm.html(newContent);
        
        next.shift();
        
        if (next.length > 0) {
            let animationSpeed = (Math.floor(Math.random() * 10 ) + 10);
            setTimeout(function() {
                fn(fn, elm, next, pre, post, callback)
            }, animationSpeed);
        } else {
            callback();
        }
    };
    
    callBack(callBack, element, steps, prefix, postfix, oncomplete);
}

function animate_index_title() {
    let $title = $('#title');

    $title.removeClass("blinker-animate");
    
    swapText(
        $title, 
        "@author?",
        "Stephen?",
        "Who is <span class=\"blinker-highlight\">", 
        "</span>",
        function() {
            $title.addClass("blinker-animate");
        }
    );
}

$(document).ready(function() {
    setTimeout(function() {
        animate_index_title()
    }, 2000);
    
});