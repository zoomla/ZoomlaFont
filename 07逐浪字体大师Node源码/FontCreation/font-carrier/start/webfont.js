var fs = require('fs')
var options = {id:"Z01-fonts_基于逐浪字库大师(www.ziti163.com)",horizAdvX:"",vertAdvy:""}//默认大小1024
var fontCarrier = require('../lib/index.js')

var userFont = JSON.parse(fs.readFileSync("./start/fonts.json", 'utf8').toString());
console.log("已读取 " + userFont.Words.length + " 个字");

fs.unlink("./start/fonts.json", () => {});

//创建空白字体，使用svg生成字体
var font = fontCarrier.create()

// 从字体文件中解析
console.log("解析字体 " + userFont.FontFamily + " 中...");
var transFont = fontCarrier.transfer('./start/webfont/' + userFont.FontFamily);
console.log("字体解析完成...");

// 获取字体svg并设置
for (let i = 0; i < userFont.Words.length; i++) {
  let word = userFont.Words[i];
  console.log("正在添加字[" + word + "]");
  let svg = transFont.getGlyph(word);
  font.setGlyph(word, svg);
}

font.output({
  path:'./start/font/fonts/' + userFont.FontName,
  //types: ['ttf', 'eot'], //指定类型
})

console.log("字体生成成功！");