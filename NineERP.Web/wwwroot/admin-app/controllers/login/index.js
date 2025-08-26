var loginController = function () {
    this.initialize = function () {
        registerEvents();
    };

    var registerEvents = function () {
        // Toggle show/hide password
        $('.eye_icon').on('click', function () {
            var $password = $('#password');
            var isPassword = $password.attr('type') === 'password';

            $password.attr('type', isPassword ? 'text' : 'password');
            $(this).toggleClass('active', isPassword);
        });

        // Handle login click
        $('#btnLogin').on('click', function (e) {
            e.preventDefault();

            var user = $('#username').val();
            var pass = $('#password').val();

            let isValid = true;

            if (!user || user.length > 255) {
                showToast(loginResources.usernameRequired, 'error');
                isValid = false;
            }

            if (!pass || pass.length > 255) {
                showToast(loginResources.passwordRequired, 'error');
                isValid = false;
            }

            if (isValid) {
                showLoading();
                login(user, pass);
            }
        });
    };

    var login = function (user, pass) {
        $.ajax({
            type: 'POST',
            url: '/login/authenticate',
            data: {
                UserName: user,
                Password: pass,
                returnUrl: new URLSearchParams(window.location.search).get("ReturnUrl")
            },
            dataType: 'json',
            success: function (res) {
                if (res.succeeded) {
                    window.location.href = res.redirectUrl || '/dashboard';
                } else {
                    showToast(loginResources[res.messages] || loginResources.loginFailed, 'error');
                }
            },
            error: function () {
                showToast(loginResources.loginError, 'error');
            },
            complete: function () {
                hideLoading();
            }
        });
    };

    var showToast = function (message, type = 'info') {
        var color = type === 'success' ? 'bg-success' :
            type === 'error' ? 'bg-danger' :
                type === 'warning' ? 'bg-warning' : 'bg-info';

        $.toast({
            heading: type.toUpperCase(),
            text: message,
            position: 'top-right',
            loaderBg: '#fff',
            icon: type,
            hideAfter: 3000,
            stack: 3
        });
    };

    var showLoading = function () {
        $('#btnLogin')
            .html('<span class="spinner-border spinner-border-sm me-1"></span> ' + loginResources.loggingIn)
            .prop('disabled', true);
    };

    var hideLoading = function () {
        $('#btnLogin')
            .html(loginResources.login)
            .prop('disabled', false);
    };
};
