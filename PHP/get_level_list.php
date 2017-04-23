<?php 
    require("mysql_info.php");
    require("utility.php");
    
    //Strings must be escaped to prevent SQL injection attack. 
    $name = mysql_real_escape_string($_GET['n']);
    $password = $_GET['p'];
    $hash = $_GET['h'];

    $error = "";
    if(CheckHash($name, $password, $hash, $error))
    {
        $user = CheckLogin($name, $password, $error);
        if ($user != null)
        {
            //Get the lincence id
            $licence_id = $user['licence_id'];

            //Get all the levels that are linked to this licence & echo their names
            $query = "SELECT * FROM Flavour_Levels WHERE licence_id='$licence_id'";
            $result = mysql_query($query) or die($error .= "E_Query failed: " . mysql_error() ."\n");
         
            $num_results = mysql_num_rows($result);  
         
            for($i = 0; $i < $num_results; $i++)
            {
                $row = mysql_fetch_array($result);

                echo $row['id'] . ";" . $row['name'] . "\n";
            }
        }
    }

    echo $error;
?>