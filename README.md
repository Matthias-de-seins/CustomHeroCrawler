# info
This program should have been a console application and it is not fully functional if you pull it.
It needs a few small tweaks / bug fixes to make it work again from scratch. This program is only good for this website, if they change the layout, the program is fucked.

# How to use it
In the Index homecontroller, you see a few lines of code in comments. Each step needs a seperate start up and put the previous line of code back in comments. 
0) Delete all the files in wwwroot/files 
1) Make the _heroes.html file --> myCrawler.MakeHeroFiles(heronames); 
2) myCrawler.DownloadHeroes();
3) myCrawler.DownloadHero(heronames);
4) myCrawler.CreateHeroImages(heronames);
5) myCrawler.DownloadHeroImages(heronames);
6) myCrawler.CreateJsonFile();
7) myCrawler.PopulateJsonFile(heronames);

