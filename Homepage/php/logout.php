<?php
/**
 * #############################################################################
 * #                                                                           #
 * # copyright (c) 2019 marcos software, all rights reserved                   #
 * #                                                                           #
 * # this file may not be redistributed in whole or significant part           #
 * # and is subject to the marcos software license.                            #
 * #                                                                           #
 * # @author: marcos software, <info@marcos-software.de>                       #
 * # @copyright: 2018, marcos-software.de, http://www.marcos-software.de       #
 * #                                                                           #
 * #############################################################################
 */ 
    ini_set('session.cookie_lifetime', 60 * 60 * 24 * 7);  // 7 day cookie lifetime
	
	session_start();
	session_destroy();

	setcookie("identifier","",time()-(60*60*24*365), "", "", true, true);
	setcookie("securitytoken","",time()-(60*60*24*365), "", "", true, true);
	
	echo '<html lang="en">';
	
	readfile("../html/head.html");	
	readfile("../html/logout.html");
	
?>