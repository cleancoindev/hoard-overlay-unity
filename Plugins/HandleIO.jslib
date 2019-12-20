var HandleIO = {
     SyncFiles : function()
     {
		console.log('SyncFiles')
         FS.syncfs(false,function (err) {
			if (err !== null)
				console.log('SyncFiles failed with error:' + err)
			else
				console.log('SyncFiles completed')
         });
     }
};
mergeInto(LibraryManager.library, HandleIO);