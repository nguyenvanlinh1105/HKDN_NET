var locationHelper = (function () {
    let countryListRaw = [];
    let provinceListByCountry = {};
    let countryMap = {};
    let provinceMap = {};
    function initialize(callback) {
        // Chỉ load danh sách country khi khởi tạo
        $.get('/mstlocation/getcountries', function (response) {
            if (response.succeeded) {
                countryListRaw = response.data || [];
                countryListRaw.forEach(c => {
                    countryMap[c.id] = c.name;
                });
                // Load provinces cho tất cả country
                const requests = countryListRaw.map(c => {
                    return $.get("/mstlocation/getprovincesbycountry", { countryId: c.id }, function (res) {
                        if (res.succeeded) {
                            provinceListByCountry[c.id] = res.data || [];                            
                        } else {
                            provinceListByCountry[c.id] = [];
                            console.error(`Failed to load provinces for country ${c.id}`);
                        }
                    }).fail(function () {
                        provinceListByCountry[c.id] = [];
                        console.error(`Error loading provinces for country ${c.id}`);
                    });
                });

                // Chờ tất cả API hoàn tất rồi mới gọi callback
                $.when(...requests).done(function () {
                    provinceMap = {};
                    for (const countryId in provinceListByCountry) {
                        provinceListByCountry[countryId].forEach(p => {
                            provinceMap[p.id] = p.name;
                        });
                    }
                    if (typeof callback === 'function') callback();
                });

            } else {
                console.error('Failed to load countries.');
            }
        }).fail(function () {
            console.error('Error loading countries.');
        });
    }
    function loadCountries($ddlCountry, $ddlProvince, selectedCountryId, selectedProvinceId, localization) {
        let html = `<option value="">-- ${localization?.SelectCountry || 'Select country'} --</option>`;
        countryListRaw.forEach(c => {
            html += `<option value="${c.id}" ${c.id == selectedCountryId ? 'selected' : ''}>${c.name}</option>`;
        });
        $ddlCountry.html(html);

        if (selectedCountryId) {
            loadProvinces($ddlProvince, selectedCountryId, selectedProvinceId, localization);
            $ddlProvince.prop('disabled', false);
        } else {
            $ddlProvince.html(`<option value="">-- ${localization?.SelectProvince || 'Select province'} --</option>`).prop('disabled', true);
        }
    }
    function loadProvinces($ddlProvince, countryId, selectedProvinceId, localization) {
        if (!countryId) {
            $ddlProvince.html(`<option value="">-- ${localization?.SelectProvince || 'Select province'} --</option>`).prop('disabled', true);
            return;
        }
        $.get("/mstlocation/getprovincesbycountry", { countryId: countryId }, function (response) {
            if (response.succeeded) {
                provinceListByCountry[countryId] = (response.data || []).sort((a, b) => a.name.localeCompare(b.name));
                let html = `<option value="">-- ${localization?.SelectProvince || 'Select province'} --</option>`;
                provinceListByCountry[countryId].forEach(p => {
                    html += `<option value="${p.id}" ${p.id == selectedProvinceId ? 'selected' : ''}>${p.name}</option>`;
                });
                $ddlProvince.html(html).prop('disabled', false);
            } else {
                $ddlProvince.html(`<option value="">-- ${localization?.SelectProvince || 'Select province'} --</option>`).prop('disabled', true);
                console.error('Failed to load provinces.');
            }
        }).fail(function () {
            console.error('Error loading provinces.');
        });
    }

    return {
        initialize,
        loadCountries,
        loadProvinces,
        getCountryMap: () => countryMap,
        getProvinceMap: () => provinceMap
    };
})();