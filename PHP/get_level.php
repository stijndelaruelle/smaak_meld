<?php 
    require("mysql_info.php");
    require("utility.php");
    
    //Strings must be escaped to prevent SQL injection attack. 
    $name = mysql_real_escape_string($_GET['n']);
    $password = $_GET['p'];
    $hash = $_GET['h'];
    $level_id = $_GET['l'];

    $error = "";
    if(CheckHash($name, $password, $hash, $error))
    {
        $user = CheckLogin($name, $password, $error);
        if ($user != null)
        {
            //Get the user's lincence id
            $user_licence_id = $user['licence_id'];

            //Get all the levels that are linked to this licence & echo their names
            $query = "SELECT * FROM Flavour_Levels WHERE id='$level_id'";
            $result = mysql_query($query) or die($error .= "E_Query failed: " . mysql_error() . "\n");
         
            $level = mysql_fetch_array($result);

            //Double check, should always succeed
            if ($user_licence_id == $level['licence_id'])
            {
                //Read the according file and show that one (will be encrypted in the future)
                $filename = "./levels/level_" . $level['licence_id'] . "_" . $level['id'] . ".json";

                echo file_get_contents($filename, FILE_USE_INCLUDE_PATH);
            }
            else
            {
                $error .= "Licence validation error.\n";
            }
        }
    }

    echo $error;
?>