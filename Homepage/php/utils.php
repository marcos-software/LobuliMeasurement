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

	function getSalt() {
		
		return 'YOUR_SALT_HERE';	
	}

	function random_string() {

		if(function_exists('random_bytes')) 
		{
			$bytes = random_bytes(16);
			$str = bin2hex($bytes); 
			
		} else if(function_exists('openssl_random_pseudo_bytes')) {
			
			$bytes = openssl_random_pseudo_bytes(16);
			$str = bin2hex($bytes); 
		} else if(function_exists('mcrypt_create_iv')) {
			
			$bytes = mcrypt_create_iv(16, MCRYPT_DEV_URANDOM);
			$str = bin2hex($bytes); 
			
		} else { 
		
			$str = md5(uniqid(getSalt(), true));
			
		} 
		
		return $str;
	}
?>