$(document).ready(function () {



    $(document).on("click", ".star", function (e) {
        e.preventDefault();

        var $clickedStar = $(this);
        var productId = $clickedStar.data("product-id");
        var clickedRating = $clickedStar.data("rating");

        $.ajax({
            type: "POST",
            url: "/Product/Rate",
            data: { productId: productId, rating: clickedRating },
            success: function (data) {
                updateStarIcons(productId, clickedRating);
            },
            error: function () {
                console.error("Error in AJAX request");
            }
        });
    });

    function updateStarIcons(productId, clickedRating) {
        var $stars = $(".star[data-product-id='" + productId + "']");
        $stars.each(function () {
            var $star = $(this);
            var starRating = $star.data("rating");
            var isSolid = starRating <= clickedRating;
            $star.find("i").toggleClass("fa-solid", isSolid).toggleClass("fa-regular", !isSolid);
        });
    }

  
    $('#searchBtn').click(function (e) {
        e.preventDefault();

        let selectedCategoryId = $('#categoryID li.active').attr('id');
        let selectedCategory = $('#categoryID li.active').text();
        let search = $('#searchInput').val().trim();

        let searchUrl = '/product/search?search=' + search + '&categoryId=' + selectedCategoryId;

        if (search.length >= 3) {

            fetch(searchUrl)
                .then(res => res.text())
                .then(data => {
                    console.log(data);

                    $('#searchBody').html(data)
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        };
    });




    $('#categoryID li').click(function () {
        $('#categoryID li').removeClass('active');
        $(this).addClass('active');
        let selectedCategory = $(this).text();
        $('div.select-option span').text(selectedCategory);
    });

 
    $(function () {
        $(".fold-table tr.view").on("click", function () {
            if ($(this).hasClass("open")) {
                $(this).removeClass("open").next(".fold").removeClass("open");
            } else {
                $(".fold-table tr.view").removeClass("open").next(".fold").removeClass("open");
                $(this).addClass("open").next(".fold").addClass("open");
            }
        });
    });


    $('.addAdressBtn').click(function (e) {
        e.preventDefault();

        $('.addAdress').removeClass('d-none');
        $('.addAdressBtn').addClass('d-none');
        $('.adressDiv').addClass('d-none');
    })


    $('.goBack').click(function (e) {
        e.preventDefault();

        $('.addAdress').addClass('d-none');
        $('.addAdressBtn').removeClass('d-none');
        $('.adressDiv').removeClass('d-none');
    })

    $('.editAdressBtn').click(function (e) {
        e.preventDefault();

        $('.adressDiv').addClass('d-none');
        $('.addAdressBtn').addClass('d-none');
        $('.editAdress').removeClass('d-none');

        let url = $(this).attr('href');

        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.editAdress').html(data);
            })
    });

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }



    $('.wishBtn').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.wishlist_inner').html(data);

                toastr["success"]("Added to Wish...");
            })
    });
  


    $('.addBasket').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.mini_cart').html(data);

                toastr["success"]("Was added...");
            })
    });



    let successToaster = $('#successToaster').val();

    if (successToaster.val() != undefined && successToaster.val().length > 0) {
        toastr["success"](successToaster.val())
    }

    let warningToaster = $('#warningToaster').val();

    if (warningToaster.val() != undefined && warningToaster.val().length > 0) {
        toastr["warning"](warningToaster.val())
    }

});


