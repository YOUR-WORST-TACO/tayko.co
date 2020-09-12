function animate_index_0() {
    let $element = $('#index-whoami-0');

    swapText(
        $element,
        "",
        "I am the detective,",
        "",
        "",
        function() {
            setTimeout(function() {
                animate_index_1();
            }, 500);
        },
        30
    );
}

function animate_index_1() {
    let $element = $('#index-whoami-1');

    swapText(
        $element,
        "",
        "the murderer,",
        "",
        "",
        function() {
            setTimeout(function() {
                animate_index_2();
            }, 500);
        },
        30
    );
}
function animate_index_2() {
    let $element = $('#index-whoami-2');

    swapText(
        $element,
        "",
        "and the narrator.",
        "",
        "",
        function() {
            setTimeout(function() {
                animate_index_3();
            }, 1000);
        },
        30
    );
}
function animate_index_3() {
    let $element = $('#index-whoami-3');
    swapText(
        $element,
        "",
        "I am a programmer.",
        "",
        "",
        function() {
            animate_arrow();
        },
        30
    );
}

function animate_arrow() {
    let $arrow = $('#index-arrow');
    
    $arrow.css("display", "block");
    $arrow.addClass("arrow-animate");
}

function animate_index_title() {
    let $title = $('#title');

    $title.removeClass("blinker-animate");

    let $startText = $('.blinker-highlight');

    swapText(
        $title,
        $startText.text(),
        "Stephen?",
        "Who is <br/> <span class=\"blinker-highlight\">",
        "</span>",
        function() {
            $title.addClass("blinker-animate");

            setTimeout(function()
            {
                $title.removeClass("blinker");
                animate_index_0();
            }, 2000);
        }
    );
}

$(document).ready(function() {
    setTimeout(function() {
        animate_index_title()
    }, 2000);
});