//fo v.ziti163.com write 2020-8-23
//Powerd by Zoomla!逐浪CMS
//免费下载：www.z01.com
var timestamp = 0;
var mouseSpeed = 0;
var isReset = false;
function Point(x, y) {
    this.x = x || 0;
    this.y = y || 0;
}
Point.prototype.x = null;
Point.prototype.y = null;
Point.prototype.clone = function () {
	return new Point(this.x, this.y);
};
var hwrite =
{
    db:
    {
        moveFlag: false, pen: null, penName: "", direction: "", strokes: [], strokes_pop: [], points: [],
        pos_now: new Point(0,0), pos_start: new Point(0,0), ctx: null, canvas: null,
    },
    style: {
        size: 26, fill: "rgba(0,0,0,1)",
    },
    pens:
    {
        "regular":
        {
            draw: function (start, end, size) {
                var ctx = hwrite.db.ctx;
                ctx.save();
                ctx.beginPath();
                ctx.moveTo(start.x, start.y);
                ctx.lineWidth = size;
                ctx.lineCap = "round";//设置或返回线条的结束端点样式
                ctx.lineJoin = "round";//设置或返回两条线相交时，所创建的拐角类型
                ctx.strokeStyle = hwrite.style.fill;
                ctx.lineTo(end.x, end.y);
                ctx.stroke();
                ctx.closePath();
                ctx.restore();
            }
        },
        "crayon":
        {
            draw: function (start, end, size) {
                var ctx = hwrite.db.ctx;
                var img = document.getElementById('pen_crayon');
                var pat = ctx.createPattern(img, 'repeat');
                ctx.save();
                ctx.beginPath();
                ctx.moveTo(start.x, start.y);
                ctx.lineWidth = size;
                ctx.lineCap = 'round';
                ctx.lineJoin = "round";
                ctx.strokeStyle = pat;
                ctx.lineTo(end.x, end.y);
                ctx.stroke();
                ctx.restore();
            }
        },
        "shoujin":
        {
            draw: function (start, end, size) {
                var ref = hwrite;
                var ctx = ref.db.ctx;
                var radius = Math.ceil(size / 2);

                ctx.save();
                ctx.beginPath();
                ctx.moveTo(start.x, start.y);
                ctx.lineWidth = size;
                ctx.lineCap = 'round';
                ctx.lineJoin = "round";
                ctx.strokeStyle = hwrite.style.fill;
                ctx.lineTo(end.x, end.y);
                ctx.stroke();
                switch (ref.db.direction) {
                    case "right":
                        ctx.fillStyle = hwrite.style.fill;
                        ctx.moveTo(end.x, end.y);
                        ctx.quadraticCurveTo(end.x, end.y - radius * 2, end.x + size, end.y);
                        ctx.moveTo(end.x, end.y);
                        ctx.quadraticCurveTo(end.x, end.y + radius * 2, end.x + size, end.y);
                        ctx.fill();
                        break;
                    case "left":
                    case "top":
                    case "bottom":
                    case "left-top":
                    case "left-bottom":
                    case "right-top":
                    case "right-bottom":
                    default:
                        break
                }
                ctx.closePath();
                ctx.restore();
            }
        },
        "pencil":
        {
            draw: function (start, end, size) {
                var ctx = hwrite.db.ctx;
                var img = document.getElementById('pen_pencil');
                var pat = ctx.createPattern(img, 'repeat');
                ctx.save();
                ctx.beginPath();
                ctx.moveTo(start.x, start.y);
                ctx.lineWidth = size;
                ctx.lineCap = 'round';
                ctx.lineJoin = "round";
                ctx.strokeStyle = pat;
                ctx.lineTo(end.x, end.y);
                ctx.stroke();
                ctx.restore();
            }
        },
        "brush":
        {
            draw: function (start, end, size) {
                var ctx = hwrite.db.ctx;
                var img = document.getElementById('pen_brush');
                var pat = ctx.createPattern(img, 'repeat');
                ctx.save();
                ctx.beginPath();
                ctx.moveTo(start.x, start.y);
                ctx.lineWidth = size;
                ctx.lineCap = 'round';
                ctx.lineJoin = "round";
                ctx.strokeStyle = pat;
                ctx.lineTo(end.x, end.y);
                ctx.stroke();
                ctx.restore();
            }
        },
        "xingshu":
        {
            draw: function (start, end, size) {
                var ref = hwrite;
                var ctx = ref.db.ctx;
                ref.pens["regular"].draw(start, end, Math.round(size / 3))
            }
        },
    },
    fonts: {
        'none': '常规',
        'zfont01': '逐浪报人书法行体',
        'zfont02': '逐浪海棠居刻本字',
        'zfont03': '逐浪粗颜楷',
        'zfont04': '逐浪日系楷体',
        'zfont05': '逐浪唐寅行书体',
        'zfont06': '逐浪帅宋斜楷体',
        'zfont07': '逐浪音乐符号歌谱体',
        'zfont08': '逐浪瑶小硬',
        'zfont09': '逐浪丫玉体',
        'zfont10': '逐浪新宋_特细',
        'zfont11': '逐浪湘教钢笔体',
        'zfont12': '逐浪细阁体',
        'zfont13': '逐浪文宣剪纸体',
        'zfont14': '逐浪时尚钢笔体',
        'zfont15': '逐浪金农书法体',
        'zfont16': '逐浪海昏侯汉简隶书',
        'zfont17': '逐浪古宋书法楷体',
        'zfont18': '逐浪盖世英雄狂草书',
        'zfont19': '逐浪报人书法行体',
        'zfont20': '逐浪仿篆体',
    },
    onmousedown: function (e) {
        var ref = hwrite;
        var db = ref.db;
        db.moveFlag = true;
        db.pos_start = ref.getPos(e);
    },
    onmouseup: function (e) {
        var ref = hwrite;
        var db = ref.db;
        db.moveFlag = false;
        if (db.points.length > 0) {
            db.strokes.push(db.points);
            db.points = []
        }
        isReset = false;
        db.strokes_pop = [];
    },
    onmouseout: function (e) {
        this.onmouseup(e)
    },
    onmousemove: function (e) {
        var ref = hwrite;
        if (!ref.db.moveFlag) {
            return;
        }
        var db = ref.db;
        var style = ref.style;
        var pos_now = ref.getPos(e);
        var distance = ref.distance(db.pos_start, pos_now);
        if (distance < 1) {
            return;
        }

        // 增加鼠标速度计算
        var now = Date.now();
        if(timestamp != 0){
            var span = now - timestamp;
            mouseSpeed = Math.round(distance / span * 1000);
            //console.log(span, distance, mouseSpeed);
        }
        timestamp = now;
            
        if (pos_now && (pos_now.x != db.pos_start.x || pos_now.y != db.pos_start.y)) {
            var horizontal = "", vertical = "", direction = "";
            if (pos_now.x < db.pos_start.x) {
                horizontal = "left"
            }
            else if (pos_now.x > db.pos_start.x) {
                horizontal = "right"
            }
            if (pos_now.y < db.pos_start.y) {
                vertical = "top"
            }
            else if (pos_now.y > db.pos_start.y) {
                vertical = "bottom"
            }
            if (horizontal == "") {
                direction = vertical
            }
            else if (vertical == "") {
                direction = horizontal
            }
            else {
                direction = horizontal + "-" + vertical
            }
            db.direction = direction
        }
        var size = hwrite.getSize(ref.style.size, mouseSpeed);
        ref.db.pen.draw(db.pos_start, pos_now, size);
        db.points.push({
            "pen": ref.db.penName, time: new Date().getTime(), "start": db.pos_start.clone(), "end": pos_now.clone(), "size": size
        });
        db.pos_start = pos_now;
    },
    init: function (id) {
        var ref = this;
        ref.db.canvas = document.getElementById(id);
        ref.db.ctx = ref.db.canvas.getContext("2d");
        ref.db.ctx.fillStyle = ref.style.fill;
        ref.db.canvas.onmousedown = ref.onmousedown;
        ref.db.canvas.onmousemove = ref.onmousemove;
        ref.db.canvas.onmouseup = ref.onmouseup;
        ref.db.canvas.onmouseout = ref.onmouseout;
        $("#draw_pre").click(ref.preStep);
        $("#draw_next").click(ref.nextStep);
        $("#draw_clear").click(ref.clear);
        $("#draw_replay").click(ref.replay);
        $(".brush_list a").click(function () {
            ref.choosePen($(this).data("pen"));
        });
        if (localStorage["hwrite_pen"]) {
            ref.choosePen(localStorage["hwrite_pen"]);
        }
        else { ref.choosePen("regular"); }
            
        $("#font_select").on('change', function () {
            ref.chooseFont($("#font_select option:selected").data('font'));
        });
        if (localStorage["bg_font_family"]) {
            ref.chooseFont(localStorage["bg_font_family"]);
        }
        else { ref.chooseFont("none"); }

        $("#pen_size").on('change', function () {
            ref.choosePenSize($(this).val());
        });
        if (localStorage["pen_size"]) {
            ref.choosePenSize(localStorage["pen_size"]);
        }
        else { ref.choosePenSize("26"); }
    },
    getPos: function (e) {
        var canvas = hwrite.db.canvas;
        var bound = canvas.getBoundingClientRect();
        var x = e.clientX - bound.left + (document.body.scrollLeft || document.documentElement.scrollLeft);
        var y = e.clientY - bound.top + (document.body.scrollTop || document.documentElement.scrollTop);
        return new Point(x, y) ;
    },
    distance: function (a, b) {
        var x = b.x - a.x, y = b.y - a.y;
        return Math.sqrt(x * x + y * y);
    },
    getSize: function(size, speed){                    
        // log线条宽度衰减由快到慢
        // var st = Math.log10(speed);
        // st = (st < 0 ? 0 : (st > 4 ? 4 : st)) / (4 / 0.5)
        // var wsize = size * (1 - st);

        // sqrt线条宽度衰减由慢到快
        var st = Math.sqrt(speed);
        st = (st > 100 ? 100 : st) / (100 / 0.8)
        var wsize = size * (1 - st);

        return wsize;
    },
    getSvg() {
        var ref = hwrite;
        var db = ref.db;
        db.ctx = C2S(db.canvas.width, db.canvas.height);
        if (db.strokes.length < 1)
            return null;
        db.strokes.forEach(function (item) {
            if (!item || item.length < 1) {
                return;
            }
            item.forEach(function (point) {
                ref.pens[point.pen + ""].draw(point.start, point.end, point.size);
            });
        });
        var svg = db.ctx.getSerializedSvg();
        db.ctx = db.canvas.getContext("2d");
        return svg;
    },
    replay: function () {
        var ref = hwrite;
        var db = ref.db;
        if (db.strokes.length < 1) {
            return
        }
        var canvas = ref.db.canvas;
        ref.db.ctx.clearRect(0, 0, canvas.width, canvas.height);
        var time = 10;
        var index = 1;
        db.strokes.forEach(function (item) {
            if (!item || item.length < 1) {
                return
            }
            item.forEach(function (point) {
                setTimeout(function () {
                    ref.pens[point.pen + ""].draw(point.start, point.end, point.size)
                },
                (index * time));
                ++index
            })
        })
    },
    clear: function () {
        isReset = true;
        var ref = hwrite;
        var canvas = ref.db.canvas;
        ref.db.ctx.clearRect(0, 0, canvas.width, canvas.height);
        ref.db.strokes_pop = JSON.parse(JSON.stringify(ref.db.strokes));
        ref.db.strokes = [];
    },
    preStep: function () {
        var ref = hwrite;
        if (isReset) {
            ref.db.strokes = JSON.parse(JSON.stringify(ref.db.strokes_pop));
            ref.db.strokes_pop = [];
            isReset = false;
            ref.renderByStrokes();
        }
        else if(ref.db.strokes.length > 0){
            ref.db.strokes_pop.push(ref.db.strokes.pop());
            ref.renderByStrokes();
        }
    },
    nextStep: function () {
        var ref = hwrite;
        var db = ref.db;
        if (db.strokes_pop.length < 1) {
            return;
        }
        db.strokes.push(db.strokes_pop.pop());
        ref.renderByStrokes()
    },
    renderByStrokes: function () {
        var ref = hwrite;
        var canvas = ref.db.canvas;
        ref.db.ctx.clearRect(0, 0, canvas.width, canvas.height);
        if (ref.db.strokes.length < 1) {
            return
        }
        ref.db.strokes.forEach(function (item, index, list) {
            if (!item || item.length < 1) {
                return
            }
            var pen = ref.pens[item[0].pen];
            for (var i = 0; i < item.length; i++) {
                pen.draw(item[i].start, item[i].end, item[i].size)
            }
        })
    },
    choosePen: function (name) {
        hwrite.db.pen = this.pens[name];
        hwrite.db.penName = name;
        localStorage.setItem("hwrite_pen", name);

        $(".brush_list a").removeClass("active");
        $(".brush_list a").each(function () {
            if ($(this).data("pen") == name) { $(this).addClass("active"); }
        });
    },
    chooseFont: function (font) {
        localStorage.setItem("bg_font_family", font);
        $("#bg_word").css("font-family", font);
        $("#word_show").css("font-family", font);
        $("#font_select option").each(function () {
            if ($(this).data("font") == font) { $(this).attr('selected','selected'); }
        });
    },
    choosePenSize: function (size) {
        localStorage.setItem("pen_size", size);
        this.style.size = size;
        $("#pen_size").val(size);
        $("#pen_size_view").text(size);
        $("#pen_size_view").css("width", size + 'px');
    }
};
hwrite.init("canvas_main");
//小字提示
$("#btn_show_word").click(function () {
    var b = $("#btn_show_word").hasClass("font_show_word_bg");
    if (b) {
        $("#btn_show_word").removeClass("font_show_word_bg");
        $("#icon-show_word").removeClass("d-none");
        $("#bg_word").removeClass("d-none");
        $("#word_show").addClass("d-none");
    }
    else {
        $("#btn_show_word").addClass("font_show_word_bg");
        $("#icon-show_word").addClass("d-none");
        $("#bg_word").addClass("d-none");
        $("#word_show").removeClass("d-none");
    }
});
$(document).ready(function () {        
    $("#bg_word").text("\永");
    $("#word_show").text("\永");
    $("#canvas_bg").text("\永");
    $("#font_canvas_bg").text("\永");
    //var link = window.location.protocol + "//" + window.location.host + "/font/wordlist?id=" + window.location.href.replace(/(\S*writeId=)|(&word\S*)/gi, "")
    //$("#gotolist").attr("href", link);
});
$(document).on("keydown", (e) => {
    if (e.ctrlKey && e.which == 90)
        $("#draw_pre").click();
    else if (e.ctrlKey && e.which == 89)
        $("#draw_next").click();
});