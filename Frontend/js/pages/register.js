document.addEventListener('DOMContentLoaded', () => {
    const registerForm = document.getElementById('registerForm');
    const nameInput = document.getElementById('name');
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    const registerMessage = document.getElementById('registerMessage');

    // تفعيل إظهار/إخفاء كلمة المرور وتأكيدها
    setupPasswordToggle('password', 'togglePassword');
    setupPasswordToggle('confirmPassword', 'toggleConfirmPassword');

    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const name = nameInput.value.trim();
            const email = emailInput.value.trim();
            const password = passwordInput.value;
            const confirmPassword = confirmPasswordInput.value;

            // التحقق من تطابق كلمات المرور
            if (password !== confirmPassword) {
                showError('Passwords do not match!');
                return;
            }

            // التحقق من طول كلمة المرور
            if (password.length < 6) {
                showError('Password must be at least 6 characters long.');
                return;
            }

            // إخفاء أي خطأ سابق
            hideError();

            const userData = {
                name: name,
                email: email,
                password: password
            };

            const submitBtn = document.getElementById('registerBtn');
            const originalBtnContent = submitBtn.innerHTML;

            try {
                // تأثير التحميل على الزر
                submitBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> <span>Creating Account...</span>';
                submitBtn.disabled = true;

                // إرسال الطلب إلى الـ API
                const response = await fetch('https://localhost:8000/api/users', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(userData)
                });

                // معالجة الاستجابة سواء كانت JSON أو نص عادي (Plain Text) لتجنب أخطاء 409
                let errorMessage = 'Failed to register. Please try again.';
                const contentType = response.headers.get("content-type");
                let responseData = {};

                if (contentType && contentType.includes("application/json")) {
                    responseData = await response.json().catch(() => ({}));
                    errorMessage = responseData.message || responseData.title || responseData.error || errorMessage;
                } else {
                    const textData = await response.text().catch(() => '');
                    if (textData) {
                        errorMessage = textData;
                    }
                }

                if (!response.ok) {
                    // تخصيص رسالة خطأ الإيميل الموجود مسبقاً (حالة 409 Conflict)
                    if (response.status === 409 || errorMessage.toLowerCase().includes('email already exists')) {
                        errorMessage = 'This email is already registered. Please sign in or use a different email.';
                    }
                    throw new Error(errorMessage);
                }

                // تخزين التوكن في حال أرسله السيرفر
                if (responseData.token) {
                    localStorage.setItem('authToken', responseData.token);
                }

                // رسالة النجاح عبر SweetAlert2
                await Swal.fire({
                    icon: 'success',
                    title: 'Account Created Successfully!',
                    text: 'Welcome to Deal Real Estate',
                    confirmButtonColor: '#0f8c4d',
                    timer: 1500,
                    showConfirmButton: false
                });

                // التوجيه إلى الصفحة الرئيسية
                window.location.href = '../pages/home.html';

            } catch (error) {
                console.error('Registration Error:', error);
                showError(error.message || 'Something went wrong. Please check your connection.');
                
                // إعادة الزر لحالته الطبيعية عند الخطأ
                submitBtn.innerHTML = originalBtnContent;
                submitBtn.disabled = false;
            }
        });
    }

    // دالة إظهار رسائل الخطأ في الواجهة
    function showError(message) {
        if (registerMessage) {
            registerMessage.textContent = message;
            registerMessage.style.display = 'block';
            registerMessage.style.background = '#fee2e2';
            registerMessage.style.color = '#991b1b';
            registerMessage.style.padding = '10px 14px';
            registerMessage.style.borderRadius = '8px';
            registerMessage.style.marginBottom = '15px';
            registerMessage.style.fontSize = '13px';
            registerMessage.style.textAlign = 'center';
        }
    }

    // دالة إخفاء رسائل الخطأ
    function hideError() {
        if (registerMessage) {
            registerMessage.style.display = 'none';
            registerMessage.textContent = '';
        }
    }

    // دالة التحكم بإظهار وإخفاء كلمة المرور
    function setupPasswordToggle(passwordId, toggleId) {
        const passwordField = document.getElementById(passwordId);
        const toggleBtn = document.getElementById(toggleId);

        if (passwordField && toggleBtn) {
            toggleBtn.addEventListener('click', (event) => {
                event.preventDefault(); // لمنع إرسال الفورم بالخطأ عند الضغط على زر العرض
                
                const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
                passwordField.setAttribute('type', type);
                
                const icon = toggleBtn.querySelector('i');
                if (icon) {
                    icon.classList.toggle('fa-eye');
                    icon.classList.toggle('fa-eye-slash');
                }
            });
        }
    }
});