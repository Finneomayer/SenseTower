FOR /d /r %%G in (.\*) DO (
	cd %%~dpG
        build.cmd
	cd..
    )
