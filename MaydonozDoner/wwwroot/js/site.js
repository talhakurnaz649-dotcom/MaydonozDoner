$(document).ready(function () {
    updateCartBadge();

    $(document).on('click', '.btn-add-to-cart', function () {
        const id = $(this).data('id');
        const name = $(this).data('name');
        const price = parseFloat($(this).data('price'));
        const imageUrl = $(this).data('image');

        addToCart(id, name, price, imageUrl);
        
        const btn = $(this);
        const originalText = btn.html();
        btn.html('<i class="bi bi-check-circle-fill me-1"></i> Eklendi!');
        btn.removeClass('btn-order').addClass('btn-success');
        setTimeout(function () {
            btn.html(originalText);
            btn.removeClass('btn-success').addClass('btn-order');
        }, 1000);
    });

    $('#openCartBtn, .open-cart-link').on('click', function (e) {
        e.preventDefault();
        renderCartModal();
        $('#cartModal').modal('show');
    });

    $(document).on('input', '#checkoutCardNumber, #cardNumberInput', function () {
        var val = this.value.replace(/\D/g, ''); 
        if (val.length > 16) {
            val = val.substring(0, 16);
        }
        var formatted = val.match(/.{1,4}/g)?.join(' ') || val;
        this.value = formatted;
    });

    $(document).on('input', '#checkoutCardExpiry, #cardExpiryInput', function () {
        var val = this.value.replace(/\D/g, ''); 
        if (val.length > 4) {
            val = val.substring(0, 4);
        }
        var formatted = val;
        if (val.length > 2) {
            formatted = val.substring(0, 2) + '/' + val.substring(2);
        }
        this.value = formatted;
    });

    $(document).on('input', '#checkoutCardCvv, #cardCvvInput', function () {
        var val = this.value.replace(/\D/g, '');
        if (val.length > 3) {
            val = val.substring(0, 3);
        }
        this.value = val;
    });

    $(document).on('click', '#btnOrderComplete', function () {
        completeCheckout();
    });
});

function getCart() {
    try {
        const cartData = localStorage.getItem('maydonoz_cart');
        return cartData ? JSON.parse(cartData) : [];
    } catch (e) {
        return [];
    }
}

function saveCart(cart) {
    localStorage.setItem('maydonoz_cart', JSON.stringify(cart));
    updateCartBadge();
}

function updateCartBadge() {
    const cart = getCart();
    let totalItems = 0;
    cart.forEach(item => totalItems += item.quantity);
    
    const badge = $('#cartBadge');
    if (totalItems > 0) {
        badge.text(totalItems).show();
    } else {
        badge.hide();
    }
}

function addToCart(id, name, price, imageUrl) {
    let cart = getCart();
    const existingIndex = cart.findIndex(item => item.id == id);
    if (existingIndex > -1) {
        cart[existingIndex].quantity += 1;
    } else {
        cart.push({
            id: id,
            name: name,
            price: price,
            imageUrl: imageUrl || 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=500',
            quantity: 1
        });
    }
    saveCart(cart);
}

function updateQuantity(id, newQty) {
    let cart = getCart();
    const index = cart.findIndex(item => item.id == id);
    if (index > -1) {
        if (newQty <= 0) {
            cart.splice(index, 1);
        } else {
            cart[index].quantity = parseInt(newQty);
        }
        saveCart(cart);
        renderCartModal();
    }
}

function clearCart() {
    localStorage.removeItem('maydonoz_cart');
    updateCartBadge();
}

function renderCartModal() {
    const cart = getCart();
    const modalBody = $('#cartModalBody');
    const modalFooter = $('#cartModalFooter');
    
    if (cart.length === 0) {
        modalBody.html(`
            <div class="text-center py-5">
                <i class="bi bi-cart-x text-muted" style="font-size: 4rem;"></i>
                <h4 class="mt-3 text-muted">Sepetiniz Boş</h4>
                <p class="text-muted small">Menümüzden dilediğiniz lezzetleri sepetinize ekleyebilirsiniz.</p>
                <button class="btn btn-warning text-dark fw-bold mt-2 rounded-pill px-4" data-bs-dismiss="modal">Alışverişe Başla</button>
            </div>
        `);
        modalFooter.removeClass('d-flex').addClass('d-none');
        return;
    }

    let itemsHtml = `
        <div class="table-responsive">
            <table class="table table-dark table-borderless align-middle" style="background-color: #1a1f2c;">
                <thead>
                    <tr class="text-muted border-bottom border-secondary" style="font-size: 0.85rem;">
                        <th>Ürün</th>
                        <th class="text-center">Adet</th>
                        <th class="text-end">Toplam</th>
                        <th class="text-center"></th>
                    </tr>
                </thead>
                <tbody>
    `;

    let total = 0;
    cart.forEach(item => {
        const itemTotal = item.price * item.quantity;
        total += itemTotal;
        itemsHtml += `
            <tr class="border-bottom border-secondary-subtle">
                <td style="min-width: 150px;">
                    <div class="d-flex align-items-center">
                        <img src="${item.imageUrl}" alt="${item.name}" class="rounded-3 me-2" style="width: 40px; height: 40px; object-fit: cover;" onerror="this.src='https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=500'" />
                        <div>
                            <div class="fw-bold text-white small">${item.name}</div>
                            <div class="text-warning small">${item.price.toFixed(2)} TL</div>
                        </div>
                    </div>
                </td>
                <td class="text-center" style="min-width: 100px;">
                    <div class="input-group input-group-sm justify-content-center">
                        <button class="btn btn-outline-warning btn-sm border-secondary px-2" onclick="updateQuantity(${item.id}, ${item.quantity - 1})">-</button>
                        <span class="bg-dark text-white border border-secondary px-3 py-1 text-center font-monospace" style="min-width: 35px; font-size: 0.9rem;">${item.quantity}</span>
                        <button class="btn btn-outline-warning btn-sm border-secondary px-2" onclick="updateQuantity(${item.id}, ${item.quantity + 1})">+</button>
                    </div>
                </td>
                <td class="text-end fw-bold text-warning" style="min-width: 80px;">
                    ${itemTotal.toFixed(2)} TL
                </td>
                <td class="text-center">
                    <button class="btn btn-link text-danger p-1 border-0" onclick="updateQuantity(${item.id}, 0)" title="Sil">
                        <i class="bi bi-trash-fill"></i>
                    </button>
                </td>
            </tr>
        `;
    });

    itemsHtml += `
                </tbody>
            </table>
        </div>
        
        <div class="d-flex justify-content-between align-items-center my-3 p-3 rounded border border-warning" style="background-color: #232a3b;">
            <span class="fs-5 fw-bold text-white">Toplam Tutar:</span>
            <span class="fs-4 fw-bold text-warning" id="cartTotalDisplay">${total.toFixed(2)} TL</span>
        </div>
        
        <div id="checkoutFormContainer" class="mt-4 border-top border-secondary pt-3">
            <h5 class="text-warning mb-3"><i class="bi bi-truck me-2"></i> Teslimat & Ödeme Bilgileri</h5>
            <div id="checkoutLoading" class="text-center my-3">
                <div class="spinner-border text-warning" role="status"></div>
                <span class="ms-2 text-muted">Profil bilgileriniz alınıyor...</span>
            </div>
            <div id="checkoutFields" style="display: none;">
                <form id="checkoutForm">
                    <div class="mb-3">
                        <label class="form-label text-warning fw-semibold small">Teslimat Adresi</label>
                        <textarea class="form-control bg-dark border-secondary text-white" id="checkoutAddress" rows="2" placeholder="Lütfen profilinizden adresinizi güncelleyin..." required></textarea>
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label text-warning fw-semibold small">Kart Üzerindeki İsim</label>
                            <input type="text" class="form-control bg-dark border-secondary text-white" id="checkoutCardName" placeholder="Kart Sahibi Ad Soyad" required />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label text-warning fw-semibold small">Kart Numarası</label>
                            <input type="text" class="form-control bg-dark border-secondary text-white" id="checkoutCardNumber" maxlength="25" placeholder="0000 0000 0000 0000" required />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label text-warning fw-semibold small">Son Kullanma Tarihi</label>
                            <input type="text" class="form-control bg-dark border-secondary text-white" id="checkoutCardExpiry" placeholder="AA/YY" maxlength="5" required />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label text-warning fw-semibold small">CVV</label>
                            <input type="password" class="form-control bg-dark border-secondary text-white" id="checkoutCardCvv" maxlength="3" placeholder="123" required />
                        </div>
                    </div>
                </form>
            </div>
            <div id="checkoutUnauthenticated" class="alert alert-warning border-0 rounded-3 text-dark text-center my-3" style="display: none;">
                <i class="bi bi-exclamation-triangle-fill me-2"></i>
                Siparişinizi tamamlamak ve adres bilgilerinize göre gönderim yapmak için üye girişi yapmanız gerekmektedir.
                <div class="mt-2">
                    <a href="/Account/Login" class="btn btn-sm btn-dark text-warning fw-bold px-3">Giriş Yap</a>
                    <a href="/Account/Register" class="btn btn-sm btn-outline-dark fw-bold px-3 ms-2">Kayıt Ol</a>
                </div>
            </div>
        </div>
    `;

    modalBody.html(itemsHtml);
    modalFooter.removeClass('d-none').addClass('d-flex');

    $.get('/Account/GetProfileData')
        .done(function (res) {
            $('#checkoutLoading').hide();
            if (res.success) {
                $('#checkoutAddress').val(res.address);
                $('#checkoutCardName').val(res.cardHolderName);
                $('#checkoutCardNumber').val(res.cardNumber);
                $('#checkoutCardExpiry').val(res.cardExpiry);
                $('#checkoutCardCvv').val(res.cardCvv);
                
                $('#checkoutFields').show();
                $('#checkoutUnauthenticated').hide();
                $('#btnOrderComplete').prop('disabled', false).show();
            } else {
                $('#checkoutFields').hide();
                $('#checkoutUnauthenticated').show();
                $('#btnOrderComplete').prop('disabled', true).hide();
            }
        })
        .fail(function () {
            $('#checkoutLoading').hide();
            $('#checkoutFields').hide();
            $('#checkoutUnauthenticated').show();
            $('#btnOrderComplete').prop('disabled', true).hide();
        });
}

function completeCheckout() {
    const address = $('#checkoutAddress').val();
    const cardName = $('#checkoutCardName').val();
    const cardNo = $('#checkoutCardNumber').val();
    const cardExpiry = $('#checkoutCardExpiry').val();
    const cardCvv = $('#checkoutCardCvv').val();

    if (!address || !cardName || !cardNo || !cardExpiry || !cardCvv) {
        alert('Lütfen teslimat ve ödeme bilgilerinin tamamını doldurun.');
        return;
    }

    $('#cartModalBody').html(`
        <div class="text-center py-5">
            <div class="mb-4">
                <i class="bi bi-check-circle-fill text-success" style="font-size: 5rem; display: inline-block;"></i>
            </div>
            <h3 class="text-success fw-bold">Siparişiniz Başarıyla Alındı!</h3>
            <p class="text-warning fw-semibold my-3 px-3">
                Ödemeniz güvenle tahsil edilmiştir. Leziz Maydonoz Döner\'iniz en kısa sürede hazırlayıp adresinize ulaştırmak üzere yola çıkacaktır.
            </p>
            <div class="text-warning font-monospace mt-2 fs-5">Sipariş Kodunuz: MD-${Math.floor(100000 + Math.random() * 900000)}</div>
            <button class="btn btn-warning text-dark fw-bold mt-4 px-4 rounded-pill" data-bs-dismiss="modal">Menüye Geri Dön</button>
        </div>
    `);
    $('#cartModalFooter').removeClass('d-flex').addClass('d-none');
    
    clearCart();
}
