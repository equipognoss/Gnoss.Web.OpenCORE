<?php

header("Access-Control-Allow-Origin: *");

header("Content-Type: text/plain");

if(isset($_GET['password'])){
    $password = $_GET['password'];

    echo password_hash(md5($password), PASSWORD_DEFAULT);
}
else{
    echo "ERROR";
}

?>