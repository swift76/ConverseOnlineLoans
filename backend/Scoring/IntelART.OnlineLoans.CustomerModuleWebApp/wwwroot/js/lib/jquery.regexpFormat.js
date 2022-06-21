(function($) {
  'use strict';

  /**
   * Method for selecting a range of characters in an input/textarea.
   *
   * @param int rangeStart			: Where we want the selection to start.
   * @param int rangeEnd				: Where we want the selection to end.
   *
   * @return void;
   */
  function setSelectionRange(rangeStart, rangeEnd) {
    // Check which way we need to define the text range.
    if (this.createTextRange) {
      var range = this.createTextRange();
      range.collapse(true);
      range.moveStart('character', rangeStart);
      range.moveEnd('character', rangeEnd - rangeStart);
      range.select();
    }

    // Alternate setSelectionRange method for supporting browsers.
    else if (this.setSelectionRange) {
      this.focus();
      this.setSelectionRange(rangeStart, rangeEnd);
    }
  }

  /**
   * Get the selection position for the given part.
   *
   * @param string part			: Options, 'Start' or 'End'. The selection position to get.
   *
   * @return int : The index position of the selection part.
   */
  function getSelection(part) {
    var pos = this.value.length;

    // Work out the selection part.
    part = part.toLowerCase() == 'start' ? 'Start' : 'End';

    if (document.selection) {
      // The current selection
      var range = document.selection.createRange(),
        stored_range,
        selectionStart,
        selectionEnd;
      // We'll use this as a 'dummy'
      stored_range = range.duplicate();
      // Select all text
      //stored_range.moveToElementText( this );
      stored_range.expand('textedit');
      // Now move 'dummy' end point to end point of original range
      stored_range.setEndPoint('EndToEnd', range);
      // Now we can calculate start and end points
      selectionStart = stored_range.text.length - range.text.length;
      selectionEnd = selectionStart + range.text.length;
      return part == 'Start' ? selectionStart : selectionEnd;
    } else if (typeof this['selection' + part] != 'undefined') {
      pos = this['selection' + part];
    }
    return pos;
  }

  /**
   * Substitutions for keydown keycodes.
   * Allows conversion from e.which to ascii characters.
   */
  var _keydown = {
    codes: {
      46: 127,
      188: 44,
      109: 45,
      190: 46,
      191: 47,
      192: 96,
      220: 92,
      222: 39,
      221: 93,
      219: 91,
      173: 45,
      187: 61, //IE Key codes
      186: 59, //IE Key codes
      189: 45, //IE Key codes
      110: 46, //IE Key codes
    },
    shifts: {
      96: '~',
      49: '!',
      50: '@',
      51: '#',
      52: '$',
      53: '%',
      54: '^',
      55: '&',
      56: '*',
      57: '(',
      48: ')',
      45: '_',
      61: '+',
      91: '{',
      93: '}',
      92: '|',
      59: ':',
      39: '"',
      44: '<',
      46: '>',
      47: '?',
    },
  };

  /**
   * jQuery number formatter plugin. This will allow you to format regexp on an element.
   *
   * @params proxied for format_number method.
   *
   * @return : The jQuery collection the method was called with.
   */
  $.fn.regexpFormat = function(regex_format) {
    var regex_format = new RegExp(regex_format);

    // If this element is a number, then we add a keyup
    if (this.is('input:text')) {
      // Return the jquery collection.
      return (
        this.on({
          /**
           * Handles keyup events.
           *
           * Uses 'data' object to keep track of important information.
           *
           * data.c
           * This variable keeps track of where the caret *should* be. It works out the position as
           * the number of characters from the end of the string. E.g., '1^,234.56' where ^ denotes the caret,
           * would be index -7 (e.g., 7 characters from the end of the string). At the end of both the key down
           * and key up events, we'll re-position the caret to wherever data.c tells us the cursor should be.
           * This gives us a mechanism for incrementing the cursor position when we come across decimals, commas
           * etc. This figure typically doesn't increment for each keypress when to the left of the decimal,
           * but does when to the right of the decimal.
           *
           * @param object e			: the keyup event object.s
           *
           * @return void;
           */
          'keypress.format': function(e) {
            // Define variables used in the code below.
            var $this = $(this),
              key = e.keyCode ? e.keyCode : e.which,
              chara = '', //unescape(e.originalEvent.keyIdentifier.replace('U+','%u')),
              start = getSelection.apply(this, ['start']),
              end = getSelection.apply(this, ['end']),
              val = '',
              setPos = false;

            // Webkit (Chrome & Safari) on windows screws up the keyIdentifier detection
            // for numpad characters. I've disabled this for now, because while keyCode munging
            // below is hackish and ugly, it actually works cross browser & platform.

            //						if( typeof e.originalEvent.keyIdentifier !== 'undefined' )
            //						{
            //							chara = unescape(e.originalEvent.keyIdentifier.replace('U+','%u'));
            //						}
            //						else
            //						{

            if (e.shiftKey && _keydown.shifts.hasOwnProperty(key)) {
              //get shifted keyCode value
              chara = _keydown.shifts[key];
            }

            if (chara == '') chara = String.fromCharCode(key);
            //						}

            if (
              // Allow control keys to go through... (delete, backspace, tab, enter, escape etc)
              key == 8 ||
              key == 127 ||
              key == 9 ||
              key == 27 ||
              key == 13 ||
              // Allow: Ctrl+A, Ctrl+R, Ctrl+P, Ctrl+S, Ctrl+F, Ctrl+H, Ctrl+B, Ctrl+J, Ctrl+T, Ctrl+Z, Ctrl++, Ctrl+-, Ctrl+0
              ((key == 65 ||
                key == 82 ||
                key == 80 ||
                key == 83 ||
                key == 70 ||
                key == 72 ||
                key == 66 ||
                key == 74 ||
                key == 84 ||
                key == 90 ||
                key == 61 ||
                key == 173 ||
                key == 48) &&
                (e.ctrlKey || e.metaKey) === true) ||
              // Allow: Ctrl+V, Ctrl+C, Ctrl+X
              ((key == 86 || key == 67 || key == 88) && (e.ctrlKey || e.metaKey) === true) ||
              // Allow: home, end, left, right
              (key >= 35 && key <= 40 && chara.match(regex_format))
            ) {
              return;
            }
            // Stop executing if the user didn't type a regex format, backspace, or delete.
            else if (key != 8 && key != 127 && !chara.match(regex_format)) {
              // We need the original keycode now...

              // But prevent all other keys.
              e.preventDefault();
              return false;
            }
          },

          /**
           * Handles keyup events, re-formatting numbers.
           *
           * @param object e			: the keyup event object.s
           *
           * @return void;
           */
          'keyup.format': function(e) {
            // Store these variables for use below.
            var $this = $(this),
              code = e.keyCode ? e.keyCode : e.which,
              chara = '', //unescape(e.originalEvent.keyIdentifier.replace('U+','%u')),
              start = getSelection.apply(this, ['start']),
              end = getSelection.apply(this, ['end']),
              setPos;

            if (_keydown.codes.hasOwnProperty(code)) {
              code = _keydown.codes[code];
            }
            if (e.shiftKey && _keydown.shifts.hasOwnProperty(code)) {
              //get shifted keyCode value
              chara = _keydown.shifts[code];
            }
            if (chara == '') chara = String.fromCharCode(code);

            // Stop executing if the user didn't type a regex format.
            if (
              this.value === '' ||
              (chara.match(regex_format) && code !== 8 && code !== 46 && code !== 110)
            )
              return;

            // Re-format the textarea.
            $this.val($this.val());
          },

          /**
           * Reformat when pasting into the field.
           *
           * @param object e 		: jQuery event object.
           *
           * @return false : prevent default action.
           */
          'paste.format': function(e) {
            // Defint $this. It's used twice!.
            var $this = $(this),
              original = e.originalEvent,
              val = null;

            // Get the text content stream.
            if (window.clipboardData && window.clipboardData.getData) {
              // IE
              val = window.clipboardData.getData('Text');
            } else if (original.clipboardData && original.clipboardData.getData) {
              val = original.clipboardData.getData('text/plain');
            }
            if (!val.match(regex_format)) {
              e.preventDefault();
              return false;
            }
          },
        })

          // Loop each element (which isn't blank) and do the format.
          .each(function() {
            var $this = $(this).data('regexp', {
              regexp: regex_format,
            });

            // Return if the element is empty.
            if (this.value === '') return;

            // Otherwise... format!!
            $this.val($this.val());
          })
      );
    }
    // Add this symbol to the element as text.    //////????? check
    return this.text($.regexpFormat.apply(window, arguments));
  };
})(jQuery);
