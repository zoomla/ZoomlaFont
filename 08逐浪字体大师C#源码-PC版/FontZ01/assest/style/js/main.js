$.ajaxSettings.async = false;
var router_list = [
    {
        path: "/",
        component: {
            template: "#index",
            data: function () {
                return {};
            },
            methods: {
            },
            created: function () {
                var ref = this;
                //ref.$parent.post("swiper_get", {order: 1 }, function (result) {
                //        ref.sp_index = result;
                //    });
            }
        }//component end;
    },
    {
        path: "/a2",
        component: {
            template: "#a2",
            data: function () {
                var ref = this;
                //ref.$route.params
                var model = {
                     nodes:[],node: { NodeID: 0, NodeName: "" }
                };
                model.node = ref.$parent.node_get(ref.$route.params.id);
                model.nodes = ref.$parent.node_list(ref.$route.params.id);
                model.nodes.unshift(model.node);
                return model;
            },
            methods: {

            }
        },
    },
	{
        path: "/apps",
        component: {
            template: "#apps",
            data: function () {
                var ref = this;
                //ref.$route.params
                var model = {
                     nodes:[],node: { NodeID: 0, NodeName: "" }
                };
                model.node = ref.$parent.node_get(ref.$route.params.id);
                model.nodes = ref.$parent.node_list(ref.$route.params.id);
                model.nodes.unshift(model.node);
                return model;
            },
            methods: {

            }
        },
    },
	{
        path: "/photo",
        component: {
            template: "#photo",
            data: function () {
                var ref = this;
                //ref.$route.params
                var model = {
                     nodes:[],node: { NodeID: 0, NodeName: "" }
                };
                model.node = ref.$parent.node_get(ref.$route.params.id);
                model.nodes = ref.$parent.node_list(ref.$route.params.id);
                model.nodes.unshift(model.node);
                return model;
            },
            methods: {

            }
        },
    },
	{
        path: "/ai",
        component: {
            template: "#ai",
            data: function () {
                var ref = this;
                //ref.$route.params
                var model = {
                     nodes:[],node: { NodeID: 0, NodeName: "" }
                };
                model.node = ref.$parent.node_get(ref.$route.params.id);
                model.nodes = ref.$parent.node_list(ref.$route.params.id);
                model.nodes.unshift(model.node);
                return model;
            },
            methods: {

            }
        },
    },
	{
        path: "/font",
        component: {
            template: "#font",
            data: function () {
                var ref = this;
                //ref.$route.params
                var model = {
                     nodes:[],node: { NodeID: 0, NodeName: "" }
                };
                model.node = ref.$parent.node_get(ref.$route.params.id);
                model.nodes = ref.$parent.node_list(ref.$route.params.id);
                model.nodes.unshift(model.node);
                return model;
            },
            methods: {

            }
        },
    },
    {
        path: "/setting",
        component: {
            template: "#setting",
            data: function () {
                var ref = this;
                //ref.$route.params
                var model = {
                     nodes:[],node: { NodeID: 0, NodeName: "" }
                };
                model.node = ref.$parent.node_get(ref.$route.params.id);
                model.nodes = ref.$parent.node_list(ref.$route.params.id);
                model.nodes.unshift(model.node);
                return model;
            },
            methods: {

            }
        },
    },
    {
        path: "/list/:id",
        component: {
            template: "#list",
            data: function () {
                var ref = this;
                //ref.$route.params
                var model = {
                     nodes:[],node: { NodeID: 0, NodeName: "" }
                };
                model.node = ref.$parent.node_get(ref.$route.params.id);
                model.nodes = ref.$parent.node_list(ref.$route.params.id);
                model.nodes.unshift(model.node);
                return model;
            },
            methods: {
                navToContent: function (item) { this.$parent.navTo("/content/" + item.GeneralID); },
                getList:function(node){
                         var ref=this;
                         return ref.$parent.content_list(node.NodeID);
                },
            }
        },
    }
];
var app = new Vue({
    el: "#vueApp",
    router: new VueRouter({ "routes": router_list }) ,
    data: {
        nodes:[],
    },
    methods: {
        post: function (action, cfg, cb) {
            var ref = this;
            var apiUrl = "https://www.73ic.com/WXFront/API/wxmp.aspx?uid=" + ref.info.uid + "&action=";
            //$.ajax({
            //    url: apiUrl + action, type: "POST", dataType: 'jsonp', jsonp: 'callback',
            //    data: cfg,
            //    //timeout: 5000,
            //    success: function (data) {
            //        var model = APIResult.getModel(data);
            //        if (APIResult.isok(model) && typeof (cb) == "function") { cb(model.result); }
            //        else { console.error(model.retmsg); }
            //    },
            //    error: function (XMLHttpRequest, textStatus, errorThrown) {
            //        console.log("post failed");
            //    }
            //});
        },
        loadJson: function (name, cb) {
            $.get("https://www.z01.com/API/WXAPP?apiId=SSY5UF6SUK&apiKey=sZ2Mx7BvAgEWr7i6JDh4vPLFluMuR4pn&action=" + name, {}, cb);
        },
        navTo: function (url) {
            this.$router.push(url);
        },
        node_list: function (pid) {
            var ref = this;
            var result = [];
            for (var i = 0; i < ref.nodes.length; i++) {
                if (ref.nodes[i].ParentID == pid) { result.push(ref.nodes[i]); }
            }
            return result;
        },
        node_get: function (id) {
            var ref = this;
            for (var i = 0; i < ref.nodes.length; i++) {
                if (ref.nodes[i].NodeID == id) { return ref.nodes[i]; }
            }
            return {NodeID:0,NodeName:"none"};
        },
        content_list: function (nodeId) {
            var ref = this;
            var result = [];
            for (var i = 0; i < ref.list.length; i++) {
                if (ref.list[i].NodeID == nodeId) { result.push(ref.list[i]); }
            }
            return result;
        },
        content_get: function (id) {
            var ref = this;
            for (var i = 0; i < ref.list.length; i++) {
                if (ref.list[i].GeneralID == id) { return ref.list[i]; }
            }
            return { GeneralID: 0, Title: "none" };
        },
        onViewIn: function () { }
    },
    created: function () {
        var ref = this;
        ref.loadJson("node_list", function (data) { ref.nodes = data; });
        ref.loadJson("node_list", function (data) { ref.list = data; });
    },
    beforeCreate() { },
});