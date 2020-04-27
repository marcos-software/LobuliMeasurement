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
 * # @copyright: 2019, marcos-software.de, http://www.marcos-software.de       #
 * #                                                                           #
 * #############################################################################
 */ 
 
	function checkReCaptcha($response) {
		
		$secret      = 'YOUR_RECAPTCHA_SECRET_HERE';
		$remoteip    = $_SERVER['REMOTE_ADDR'];
		$baseurl     = "https://www.google.com/recaptcha/api/siteverify";
		
		$url         = $baseurl.'?secret='.$secret.'&response='.$response.'&remoteip='.$remoteip;

		$result_json = file_get_contents($url);
		$resulting   = json_decode($result_json, true);

		if($resulting['success']) {
		  
			return true;			
		} else {
			
			/* just for debug */
			// var_dump($resulting);
			
			return false;
		}			
	} 
?>