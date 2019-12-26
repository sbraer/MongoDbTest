#!/usr/bin/env bash
my_array=($(echo $MONGODB_CLUSTER_LIST))

if [ -z "$MONGODB_CLUSTER_LIST" ]; then
  echo "Nothing to do"
  exit 0
fi

for rs in "${my_array[@]}"
do
  echo "$rs..."
  while :
  do
    echo "check..."
    mongo --host $rs --eval 'db'
    if [ $? -eq 0 ]; then
      break
    fi
    sleep 2
  done
done

if [ -n "$MONGODB_USERNAME" ]; then
  username=$MONGODB_USERNAME
fi

if [ -n "$MONGODB_PASSWORD" ]; then
  password=$MONGODB_PASSWORD
fi

if [ -n "$MONGODB_USERNAME_FILE" ]; then
  username=$(cat $MONGODB_USERNAME_FILE)
fi

if [ -n "$MONGODB_PASSWORD_FILE" ]; then
  password=$(cat $MONGODB_PASSWORD_FILE)
fi

if [ -z "$username" ]; then
  echo "Missing username"
  exit 100
fi

if [ -z "$password" ]; then
  echo "Missing password"
  exit 100
fi

if [ -z "$replicasetname" ]; then
  echo "Missing replicasetname"
  exit 100
fi

clusterlength=${#my_array[@]}
status=$(mongo -u "$username" -p "$password" --host ${my_array[0]} --quiet --eval 'rs.status().members.length')
if [ "$status" != "$clusterlength" ]; then
  echo "Creating replica set"
  mongo --host ${my_array[0]} -u "$username" -p "$password" --eval 'rs.initiate({_id: "$replicasetname",version: 1, members: [{ _id: 0, host : "${my_array[0]}" }]})';
  sleep 5
  for r in "${my_array[@]}"
  do
    if [ "$r" != "${my_array[0]}" ]; then
      echo "add '$r'"
      mongo --host ${my_array[0]} -u "$username" -p "$password" --eval 'rs.add( { host: "'$r'" })';
    fi
  done
  echo "Done"
else
  echo "Already done"
fi
