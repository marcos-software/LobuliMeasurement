$(function() {
		$('.pop').on('click', function() {
			//alert("Hello! I am an alert box!!");
			$('.imagepreview').attr('src', $(this).find('img').attr('src'));
			$('#imagemodal').modal('show');   
		});		
});

function currentYear() {
    document.write(new Date().getFullYear());
}

$(document).ready(function () {

    (function ($) {

        $('#filter').keyup(function () {

            var rex = new RegExp($(this).val(), 'i');
            $('.searchable tr').hide();
            $('.searchable tr').filter(function () {
                return rex.test($(this).text().replace(/details/gi, ''));
            }).show();

        });
		
		$('a[data-toggle="tooltip"]').tooltip({
			animated: 'fade',
			placement: 'bottom',
			html: true			
		});

	}(jQuery));
});