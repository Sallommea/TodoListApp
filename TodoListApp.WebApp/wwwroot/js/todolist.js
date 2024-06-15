
    const updateButton = document.getElementById('updateButton');
    const mainContent = document.getElementById('mainContent');
    const updateFormColumn = document.getElementById('updateFormColumn');

    updateButton.addEventListener('click', function () {
        if (updateFormColumn.classList.contains('d-none')) {
            updateFormColumn.classList.remove('d-none');
            mainContent.classList.remove('col-12');
            mainContent.classList.add('col-md-6');
        } else {
            updateFormColumn.classList.add('d-none');
            mainContent.classList.remove('col-md-6');
            mainContent.classList.add('col-12');
        }
    });
