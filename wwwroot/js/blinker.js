function swapText(element = null, startText = null, endText = "", prefix = "", postfix = "", oncomplete = null, speed = 100) {
    
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
            let animationSpeed = (Math.floor(Math.random() * speed ) + speed);
            setTimeout(function() {
                fn(fn, elm, next, pre, post, callback)
            }, animationSpeed);
        } else if ( callback != null) {
            callback();
        }
    };
    
    callBack(callBack, element, steps, prefix, postfix, oncomplete);
}