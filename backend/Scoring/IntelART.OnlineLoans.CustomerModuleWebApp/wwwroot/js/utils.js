var FILE_MAX_SIZE = null;

$(document).ready(function () {
    $("#registerSSN, #registerPhone, #registerAuthenticationCode, #loginPhone").keydown(numInputKeydown);
    $("#registerSSN, #registerPhone, #registerAuthenticationCode, #loginPhone").bind('keyup change', numInputChange);
    $("#registerSSN, #registerPhone, #registerAuthenticationCode, #loginPhone").bind('paste', numInputPaste);
    $("#registerName, #registerLastName").keypress(armenianLettersInput);
    $("#registerName, #registerLastName").bind('keyup change', armenianLettersInputChange);
    $("#registerName, #registerLastName").bind('paste', armenianLettersInputPaste);
});

function openInNewTab(url) {
    var win = window.open(getAbsoluteUrl(url), '_blank');
}

var getAbsoluteUrl = (function () {
    var a;

    return function (url) {
        if (!a) a = document.createElement('a');
        a.href = url;

        return a.href;
    };
})();

function forceLogout() {
    $("#timeout").modal('show');
    $('#timeout').on('hidden.bs.modal', function () {
        $('.login')[0].click();
    });
    $('#timeout #close').on('click', function () {
        $('.login')[0].click();
    });
}

function isValidDateFormat(dateValue) {
    return (dateValue.split('/').length === 3);
}

function isValidDateYearFormat(dateValue) {
    return dateValue.split('/')[2].length === 4;
}

function isValidEmailAddress(emailAddress) {
    var pattern = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return pattern.test(emailAddress);
};

function isValidDate(date, minDate, maxDate){
	var pattern = /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/;
    if (!pattern.test(date)) {
        return false;
    }

    if (minDate && (new Date(date.split("/").reverse().join('/')) < minDate)) {
        return false;
    }

    if (maxDate && (new Date(date.split("/").reverse().join('/')) > maxDate)) {
        return false;
    }

    return true;
}

function isDateValid(date, minDate, maxDate) {
    if (minDate && (date < minDate)) {
        return false;
    }

    if (maxDate && (date > maxDate)) {
        return false;
    }

    return true;
}

function isValidNumber(num){
	
	return $.isNumeric(num);
}

function isValidPhone(phone){
	var regexp = /^[1-9]{1}[0-9]{7}$/;
	return regexp.test(phone);
}

function isValidPassport(passport){
	var regexp = /^[A-Z]{2}[0-9]{7}$/;
	return regexp.test(passport);
}

function isValidIdCard(id){
	var regexp = /^[0-9]{9}$/;
	return regexp.test(id);
}

function numInputKeydown(e) {
	// Allow: backspace, delete, tab, escape, enter
	if ($.inArray(e.keyCode, [46, 8, 9, 27, 13,  190]) !== -1 ||
		 // Allow: Ctrl+A, Command+A
		(e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) || 
		// Allow: Ctrl+V, Command+V
		(e.keyCode === 86 && (e.ctrlKey === true || e.metaKey === true)) || 
		// Allow: Ctrl+C, Command+C
		(e.keyCode === 67 && (e.ctrlKey === true || e.metaKey === true)) || 
		 // Allow: home, end, left, right, down, up
		(e.keyCode >= 35 && e.keyCode <= 40)) {
			 // let it happen, don't do anything
			 return;
	}
	// Ensure that it is a number and stop the keypress
	if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
		e.preventDefault();
	}
}

function numInputChange(event) {
	// When user select text in the document, also abort.
	var selection = window.getSelection().toString();
	if ( selection !== '' ) {
		return;
	}
	// When the arrow keys are pressed, abort.
	if ( $.inArray( event.keyCode, [38,40,37,39] ) !== -1 ) {
		return;
	}
	var $this = $( this );
	// Get the value.
	var input = $this.val();
	var input = input.replace(/[\D\s\._\-]+/g, "");
	$this.val(input);
}

function numInputPaste(event) {
	var selection = window.getSelection().toString();
	if ( selection !== '' ) {
		return;
	}
	// When the arrow keys are pressed, abort.
	if ( $.inArray( event.keyCode, [38,40,37,39] ) !== -1 ) {
		return;
	}
	var $this = $( this );
	// Get the value.
	setTimeout( function() {
		var input = $this.val();
		var input = input.replace(/[\D\s\._\-]+/g, "");
		$this.val(input);
	}, 100);
}

function armenianLettersInput(e) {
    // Allow: backspace, delete, tab, escape, enter
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 190]) !== -1 ||
        // Allow: Ctrl+A, Command+A
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl+V, Command+V
        (e.keyCode === 86 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl+C, Command+C
        (e.keyCode === 67 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
        (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is an Armenian letter and stop the keypress
    var regex = new RegExp("^[Ա-Ֆա-և-]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (!regex.test(str)) {
        e.preventDefault();
    }
}

function armenianLettersInputChange(event) {
    // When user select text in the document, also abort.
    var selection = window.getSelection().toString();
    if (selection !== '') {
        return;
    }
    // When the arrow keys are pressed, abort.
    if ($.inArray(event.keyCode, [38, 40, 37, 39]) !== -1) {
        return;
    }
    var $this = $(this);
    // Get the value.
    var input = $this.val();
    var input = input.replace(/[^Ա-Ֆա-և-]/g, "");
    $this.val(input);
}

function armenianLettersInputPaste(event) {
    var selection = window.getSelection().toString();
    if (selection !== '') {
        return;
    }
    // When the arrow keys are pressed, abort.
    if ($.inArray(event.keyCode, [38, 40, 37, 39]) !== -1) {
        return;
    }
    var $this = $(this);
    // Get the value.
    setTimeout(function () {
        var input = $this.val();
        var input = input.replace(/[^Ա-Ֆա-և-]/g, "");
        $this.val(input);
    }, 100);

}

function englishLettersInput(e) {
    // Allow: backspace, delete, tab, escape, enter
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 190]) !== -1 ||
        // Allow: Ctrl+A, Command+A
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl+V, Command+V
        (e.keyCode === 86 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: Ctrl+C, Command+C
        (e.keyCode === 67 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
        (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is an English letter and stop the keypress
    var regex = new RegExp("^[a-zA-Z-]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (!regex.test(str)) {
        e.preventDefault();
    }
}

function englishLettersInputChange(event) {
    // When user select text in the document, also abort.
    var selection = window.getSelection().toString();
    if (selection !== '') {
        return;
    }
    // When the arrow keys are pressed, abort.
    if ($.inArray(event.keyCode, [38, 40, 37, 39]) !== -1) {
        return;
    }
    var $this = $(this);
    // Get the value.
    var input = $this.val();
    var input = input.replace(/[^a-zA-Z-]/g, "");
    $this.val(input);
}

function englishLettersInputPaste(event) {
    var selection = window.getSelection().toString();
    if (selection !== '') {
        return;
    }
    // When the arrow keys are pressed, abort.
    if ($.inArray(event.keyCode, [38, 40, 37, 39]) !== -1) {
        return;
    }
    var $this = $(this);
    // Get the value.
    setTimeout(function () {
        var input = $this.val();
        var input = input.replace(/[^a-zA-Z-]/g, "");
        $this.val(input);
    }, 100);

}

function numberWithCommas (number) {
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function toUtcMidnight(date) {
    date.setHours(date.getHours() - (date.getTimezoneOffset() / 60));
    return date.toISOString();
}