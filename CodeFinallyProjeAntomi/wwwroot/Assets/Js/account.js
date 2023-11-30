$(document).ready(function(){
    $('.dashboard-list a').on('click', function(e){
        e.preventDefault(); 
        var tabId = $(this).attr('href'); 
        $('.dashboard_content .tab-pane').removeClass('show active'); 
        $(tabId).addClass('show active'); 
    });

    //$('.dashboard-list a[href="#Login.html"]').on('click', function(e){
    //    e.preventDefault(); 
    //    window.location.href = $(this).attr('href'); 
    //});
});

document.querySelectorAll('.dashboard-list li a').forEach(function (element) {
    element.addEventListener('click', function () {
        document.querySelectorAll('.dashboard-list li a').forEach(function (e) {
            e.classList.remove('active');
        });
        element.classList.add('active');
    });
});

