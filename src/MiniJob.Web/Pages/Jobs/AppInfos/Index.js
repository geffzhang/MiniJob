$(function () {
    var l = abp.localization.getResource('MiniJob');
    var createModal = new abp.ModalManager(abp.appPath + 'Jobs/AppInfos/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Jobs/AppInfos/EditModal');

    var dataTable = $('#AppInfosTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(miniJob.jobs.appInfo.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    visible: abp.auth.isGranted('MiniJob.AppInfos.Edit'),
                                    action: function (data) {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('MiniJob.AppInfos.Delete'),
                                    confirmMessage: function (data) {
                                        return l('AppInfoDeletionConfirmationMessage', data.record.appName);
                                    },
                                    action: function (data) {
                                        miniJob.jobs.appInfo
                                            .delete(data.record.id)
                                            .then(function () {
                                                abp.notify.info(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            });
                                    }
                                }
                            ]
                    }
                },
                {
                    title: l('Name'),
                    data: "appName"
                },
                {
                    title: l('Description'),
                    data: "description"
                },
                {
                    title: l('IsEnabled'),
                    data: "isEnabled"
                },
                {
                    title: l('CreationTime'), data: "creationTime",
                    render: function (data) {
                        return luxon
                            .DateTime
                            .fromISO(data, {
                                locale: abp.localization.currentCulture.name
                            }).toLocaleString(luxon.DateTime.DATETIME_SHORT);
                    }
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewAppInfoButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
