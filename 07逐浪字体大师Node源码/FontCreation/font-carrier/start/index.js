var fs = require('fs')
var options = {id:"Z01-fonts_基于逐浪字库大师(www.ziti163.com)",horizAdvX:"",vertAdvy:""}//默认大小1024
var fontCarrier = require('../lib/index.js')

var userFont = JSON.parse(fs.readFileSync("./start/fonts.json", 'utf8').toString());
console.log("已读取 " + userFont.Words.length + " 个svg文件");

fs.unlink("./start/fonts.json", () => {});

//创建空白字体，使用svg生成字体
var font = fontCarrier.create(options);

//删除字体临时文件夹
// fs.rmdirSync("./start/font/fonts");
// fs.mkdirSync("./start/font/fonts/");

console.log("将svg加入到对应字体中...");

for (let i = 0; i < userFont.Words.length; i++) {
  console.log("正在添加字[" + userFont.Words[i].Name + "]"); 
  font.setGlyph(userFont.Words[i].Name, userFont.Words[i].Svg);
  // font.fontFamily('zoomla');
}

font.output({
  path:'./start/font/fonts/' + userFont.FontName,
  //types: ['ttf', 'eot'], //指定类型
})

console.log("字体生成成功！");


