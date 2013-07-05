CWSRestart
==========

This is a small tool that can restart the cubeworld server if it's not responding. Just fill out the IPs (or click refresh to retrieve them automatically) and select an executable/batch file that should be run when the server is not responding. You can change the interval of the checks with the text box on the lower left (in seconds). Every x seconds, CWSRestart will check if the server process is still running and if its responding on the given IPs. If not, the server is shut down and the selected executable/batch file will run.
