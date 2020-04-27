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

	function getPDO($srvNr) {

		if(is_null($srvNr) || !isset($srvNr) || $srvNr == 1) {
			
			$pdo = new PDO('mysql:host=DB_IP_HERE;dbname=DB_NAME_HERE;charset=utf8', 'DB_USER_HERE', 'DB_PASS_HERE', array(PDO::MYSQL_ATTR_INIT_COMMAND => "SET NAMES 'utf8'"));
			$pdo->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);
			return $pdo;
		}
	}
?>