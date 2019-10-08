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
            let animationSpeed = (Math.floor(Math.random() * 200 ) + 100);
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
    
    let $startText = $('.blinker-highlight');
    
    swapText(
        $title, 
        $startText.text(),
        "Stephen?",
        "Who is <span class=\"blinker-highlight\">", 
        "</span>",
        function() {
            $title.addClass("blinker-animate");
            
            setTimeout(function()
            {
                $title.removeClass("blinker");  
            }, 2000);
        }
    );
}

$(document).ready(function() {
    setTimeout(function() {
        animate_index_title()
    }, 2000);
    
});