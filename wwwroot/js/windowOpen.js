
//�T�w�����j�p�A�L���b
function openwindow(url, name, iWidth, iHeight) {
    var url;     //������m;
    var name;    //�����W��;
    var iWidth;  //�������e��;
    var iHeight; //����������;
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2;  //������������m;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;   //������������m;
    var newwin = window.open(url, name, 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',status=no,location=no,status=no,menubar=no,toolbar=no,resizable=no,scrollbars=no');
    newwin.focus();
}

//�L�T�w�����j�p�A�����b
function openwindowS(url, name, iWidth, iHeight) {
    var url;     //������m;
    var name;    //�����W��;
    var iWidth;  //�������e��;
    var iHeight; //����������;
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2;  //������������m;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;   //������������m;
    var newwin = window.open(url, name, 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',status=no,location=no,status=no,menubar=no,toolbar=no,resizable=yes,scrollbars=yes');
    newwin.focus();
}