# 2025-07-03更新：去除本地yt-dlp，每次启动检测并更新yt-dlp版本，避免由于油管策略更新导致的下载失败问题。添加一个外部dll，用于解析json，dll打包进exe，启动后会释放文件并热加载dll，内置spek和TimingAnlyz，从网站到Mapping一条龙服务

# 帮朋友抓油管音频写的，本来写了个bat，但想想还是写个GUI算了

# 工具使用了[ffmpeg](https://github.com/FFmpeg/FFmpeg)和[yt-dlp](https://github.com/yt-dlp/yt-dlp)配合命令行实现
# 使用第三方库[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
# 使用第三方程序[spek](https://www.spek.cc/)
# 使用第三方程序[TimingAnlyz](https://osu.ppy.sh/users/126198)

# 可直接转换为osu的Ranked标准的192k-mp3音频
# 需要提供cookie(仅youtube)
# 可使用"Chrome浏览器"并安装"Get cookies.txt LOCALLY"插件后获取

### 使用方法
## 1. 先去Chrome浏览器获取并安装Cookie的插件，点击链接后根据提示安装即可 插件链接: https://chromewebstore.google.com/detail/get-cookiestxt-locally/cclelndahbckbenkjhflpdbgdldlbecc
![image](https://raw.githubusercontent.com/wanjiaXG/Image-Hosting-Service/main/youtube2mp3-01.png)

## 2. 安装之后打开任意油管页面，油管账号最好是登录状态，然后点击插件，选择Copy
![image](https://raw.githubusercontent.com/wanjiaXG/Image-Hosting-Service/main/youtube2mp3-02.png)
![image](https://raw.githubusercontent.com/wanjiaXG/Image-Hosting-Service/main/youtube2mp3-03.png)

## 3. 打开youtube2mp3，粘贴刚刚复制的cookie到对应的文本框中，之后复制你要下载的视频到地址栏后，点击开始下载即可，youtube2mp3链接：https://github.com/wanjiaXG/youtube2mp3/releases
![image](https://raw.githubusercontent.com/wanjiaXG/Image-Hosting-Service/main/youtube2mp3-04.png)
![image](https://raw.githubusercontent.com/wanjiaXG/Image-Hosting-Service/main/youtube2mp3-05.png)
   
