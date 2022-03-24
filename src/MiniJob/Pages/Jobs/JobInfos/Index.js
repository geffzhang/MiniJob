$(function () {
    var l = abp.localization.getResource('MiniJob');
    var createModal = new abp.ModalManager(abp.appPath + 'Jobs/JobInfos/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Jobs/JobInfos/EditModal');

    var dataTable = $('#JobInfosTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(miniJob.jobs.jobInfo.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    visible: abp.auth.isGranted('MiniJob.JobInfos.Edit'),
                                    action: function (data) {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('MiniJob.JobInfos.Delete'),
                                    confirmMessage: function (data) {
                                        return l('JobInfoDeletionConfirmationMessage', data.record.jobName);
                                    },
                                    action: function (data) {
                                        miniJob.jobs.jobInfo
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
                    data: "jobName"
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

    $('#NewJobInfoButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
