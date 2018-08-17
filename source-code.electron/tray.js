const electron = require('electron');
const path = require('path');
const app = electron.app;

// Module to create native browser window.
const BrowserWindow = electron.BrowserWindow

let tray = null;

exports.setToolTip = function(title){
  tray.setToolTip(title);
}

exports.create = function(mainWindow) {
  if (process.platform === 'darwin' || tray) {
    return;
  }

  const iconPath = path.join(__dirname, '/app/assets/bing-wallpaper-icon.png');

  const toggleApp = function(){
    if (mainWindow.isVisible()) {
      mainWindow.hide();
    } else {
      mainWindow.show();
    }
  };

  const contextMenu = electron.Menu.buildFromTemplate([
    {
      label: 'Open',
      click() {
        toggleApp();
      }
    },
    {
      type: 'separator'
    },
    {
      label: 'Exit',
      click() {
        mainWindow.forzeQuit=true;
        app.quit();
      }
    }
  ]);

  tray = new electron.Tray(iconPath);
  
  tray.setContextMenu(contextMenu);
  tray.on('double-click', toggleApp);
  
};

