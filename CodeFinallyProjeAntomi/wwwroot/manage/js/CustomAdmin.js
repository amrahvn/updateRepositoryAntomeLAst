
$(document).ready(function () {

    let isMain = $('#IsMain').is(':checked');
    if (isMain === true) {
        $('.imgContainer').removeClass('d-none');
        $('.parentContainer').addClass('d-none');
    } else {
        $('.imgContainer').addClass('d-none');
        $('.parentContainer').removeClass('d-none');
    }


    $('#IsMain').click(function () {
        let isMain = $(this).is(':checked');
        if (isMain === true) {
            $('.imgContainer').removeClass('d-none');
            $('.parentContainer').addClass('d-none');
        } else {
            $('.imgContainer').addClass('d-none');
            $('.parentContainer').removeClass('d-none');
        }
    });

    $(".toggle-checkbox").change(function () {
        var isChecked = $(this).prop("checked");

        $(".toggle-checkbox").prop("checked", false);

        if (isChecked) {
            $(this).prop("checked", true);
        }
    });

    $(document).on('click', '.deleteimageBtn', function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.filesContainer').html(data);
            })
    });

    $(document).on('click', '.setActiveBtn', function (e) {
        e.preventDefault();

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, change it!"
        }).then((result) => {
            if (result.isConfirmed) {

                let url = $(this).attr('href');

                fetch(url)
                    .then(res => res.text())
                    .then(data => {
                        $('.ListContainer').html(data);
                    })

                Swal.fire({
                    title: "Changed!",
                    text: "Your file has been changed.",
                    icon: "success"
                });
            }
        });

    })


    $(document).on('click', '.ResetBtn', function (e) {
        e.preventDefault();

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, reset it!"
        }).then((result) => {
            if (result.isConfirmed) {

                let url = $(this).attr('href');

                fetch(url)
                    .then(res => res.text())
                    .then(data => {
                        $('.ListContainer').html(data);
                    })

                Swal.fire({
                    title: "Changed!",
                    text: "Your file has been Reseted",
                    icon: "success"
                });
            }
        });

    })
   
});