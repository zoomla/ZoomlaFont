//var wpf = {};
//wpf.user_list = function() { return "[]"; }
//提供调用后台方法的接口
function callFunc() {
    //winformObj是后台声明注册的JS对象,让前端JS可调用后台方法
    //var list = winformObj.getListOfPeople();
    //for (var nLoopCnt = 0; nLoopCnt < list.length; nLoopCnt++) {
    //    var person = list[nLoopCnt];
    //}
}
function wpf_close() { wpf.close(); }
//----------------------------------------------------
//*统一传入的模型等都是angular.ToJson后的字符串,方法内部需要将其转换再处理
//*前后端用datatable的机制交互
var call = { fz: {}, user: {}, group: {}, config: {}};
call.fz.restart = function () { if (!confirm("要重启FileZilla Server服务吗?")) { return; } wpf.fz_restart(); }
call.fz.backup = function () { wpf.fz_backup(); alert("操作成功,备份路径:/config/FileZilla Server.xml"); }
//------
call.user.get = function (name) { var list = JSON.parse(wpf.user_get(name)); return list[0]; }
call.user.list = function (reload) { return JSON.parse(wpf.user_list(reload)); }
call.user.add = function (json) { return wpf.user_add(call.help.wrapJson(json)); }
call.user.update = function (json) { return wpf.user_update(call.help.wrapJson(json)); }
call.user.del = function (name) { return wpf.user_del(name); }
call.user.change = function (name, status) { return wpf.user_change(name, status); }
//-----用户权限配置(依据目录)
//call.permission = {};
//call.permission.list = function () { }
//call.permission.add = function () { }
//call.permission.update = function () { }
//call.permission.del = function () { }
//------服务器管理
call.server = {};
call.server.list = function () {
    return JSON.parse(wpf.server_list());
}
call.server.get = function (id) { return JSON.parse(wpf.server_get(Convert.ToInt(id))); }
call.server.del = function (id) { return wpf.server_del(id); }
call.server.add = function (json) { return wpf.server_add(json); }
//------
call.group.list = function () { return JSON.parse(wpf.group_list()); }
//------
call.config.get = function () { return JSON.parse(wpf.config_get()); }
//传入时已是json
call.config.update = function (json) { if (wpf.config_update(json)) { alert("配置保存成功"); } }
//------
call.sites = {};
call.sites.get = function (){
    return JSON.parse(wpf.sites_get());
}
call.sites.install = function () {
    if (!confirm("确定要安装站群Windows服务吗,请确保是以管理员身份打开当前程序")) { return false; }
    var ret = JSON.parse(wpf.sites_install());
    if (ret.retcode == 1) {
        //看命令行中提示,会遇上管理权限等问题
    }
    else { alert("提示:" + ret.retmsg); }
}
call.sites.unInstall = function () {
    if (!confirm("确定要卸载站群Windows服务吗,请确保是以管理员身份,并且卸载程序版本一致")) { return false; }
    var ret = JSON.parse(wpf.sites_unInstall());
    if (ret.retcode == 1) {
        //看命令行中提示,会遇上管理权限等问题
    }
    else { alert("提示:" + ret.retmsg); }
}
//------系统操作
call.sys = {};
call.sys.close = function () { wpf.sys_close(); }
//打开开发者工具
call.sys.opendev = function () { wpf.sys_opendev(); }
call.sys.openurl = function (url) { wpf.sys_openurl(url); }
//html table,文件名
call.sys.outToExcel = function (html, name) {
    var $table = $(html);
    $table.find(".noexcel").remove();
    $table.find("td").css("border", "1px solid #ddd");
    wpf.sys_outToExcel($table[0].outerHTML, name);
}
//------help
call.help = {};
//将angular处理后的json打包为数组,便于后端dt接收
call.help.wrapJson = function (json) {
    try {
        var arr = [];
        var model = JSON.parse(json);
        arr.push(model);
        return JSON.stringify(arr);
    } catch (ex) { alert("jsonToArr err:" + ex.message); }
}
//------用于web开发处
//var wpf = { api: "/tools/api.aspx?action=" };
//wpf.user_list = function () {
//    return JSON.stringify([{ Name: "u1", Name: "u2" }]);
//}
//wpf.group_list = function () { return JSON.stringify([{ Name: "admin", Name: "test" }]); }