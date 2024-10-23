//*****************************************************************************
//** 2641. Cousins in Binary Tree II    leetcode                             **
//*****************************************************************************


/**
 * Definition for a binary tree node.
 * struct TreeNode {
 *     int val;
 *     struct TreeNode *left;
 *     struct TreeNode *right;
 * };
 */

// A structure to store information for BFS traversal
struct QueueNode {
    struct TreeNode* node;
    int depth;
    int currval;
    int other;
};

// A queue structure to support BFS traversal
struct Queue {
    struct QueueNode** data;
    int front, rear, size, capacity;
};

// Function to create a queue
struct Queue* createQueue(int capacity) {
    struct Queue* queue = (struct Queue*)malloc(sizeof(struct Queue));
    queue->capacity = capacity;
    queue->front = queue->size = 0;
    queue->rear = capacity - 1;
    queue->data = (struct QueueNode**)malloc(capacity * sizeof(struct QueueNode*));
    return queue;
}

// Check if the queue is empty
int isEmpty(struct Queue* queue) {
    return queue->size == 0;
}

// Add an element to the queue
void enqueue(struct Queue* queue, struct TreeNode* node, int depth, int currval, int other) {
    queue->rear = (queue->rear + 1) % queue->capacity;
    queue->data[queue->rear] = (struct QueueNode*)malloc(sizeof(struct QueueNode));
    queue->data[queue->rear]->node = node;
    queue->data[queue->rear]->depth = depth;
    queue->data[queue->rear]->currval = currval;
    queue->data[queue->rear]->other = other;
    queue->size++;
}

// Remove and return the front element of the queue
struct QueueNode* dequeue(struct Queue* queue) {
    struct QueueNode* item = queue->data[queue->front];
    queue->front = (queue->front + 1) % queue->capacity;
    queue->size--;
    return item;
}

// Function to replace node values in the tree with the sum of cousin values
struct TreeNode* replaceValueInTree(struct TreeNode* root) {
    if (!root) return NULL;

    // Array to store sums at each depth level (assuming max depth of 100000)
    int sums[100000] = {0};

    // First pass: calculate sums at each depth level
    struct Queue* queue = createQueue(100000);
    enqueue(queue, root, 0, 0, 0);

    while (!isEmpty(queue)) {
        struct QueueNode* current = dequeue(queue);
        struct TreeNode* node = current->node;
        int depth = current->depth;
        
        if (node) {
            sums[depth] += node->val;
            enqueue(queue, node->left, depth + 1, 0, 0);
            enqueue(queue, node->right, depth + 1, 0, 0);
        }
        free(current);
    }

    // Second pass: update node values with cousin sums
    enqueue(queue, root, 0, root->val, 0);  // Starting with the root

    while (!isEmpty(queue)) {
        struct QueueNode* current = dequeue(queue);
        struct TreeNode* node = current->node;
        int currval = current->currval;
        int other = current->other;
        int depth = current->depth;

        // Update the node's value
        node->val = sums[depth] - currval - other;

        // Process left and right children, if they exist
        if (node->left) {
            int rightVal = node->right ? node->right->val : 0;
            enqueue(queue, node->left, depth + 1, node->left->val, rightVal);
        }
        if (node->right) {
            int leftVal = node->left ? node->left->val : 0;
            enqueue(queue, node->right, depth + 1, node->right->val, leftVal);
        }
        
        free(current);
    }

    return root;
}

// Helper function to create a new tree node
struct TreeNode* newNode(int val) {
    struct TreeNode* node = (struct TreeNode*)malloc(sizeof(struct TreeNode));
    node->val = val;
    node->left = node->right = NULL;
    return node;
}

// Function to print the tree in level-order to verify the solution
void printTree(struct TreeNode* root) {
    if (!root) return;

    struct Queue* queue = createQueue(100);
    enqueue(queue, root, 0, 0, 0);

    while (!isEmpty(queue)) {
        struct QueueNode* current = dequeue(queue);
        struct TreeNode* node = current->node;
        printf("%d ", node->val);
        if (node->left) enqueue(queue, node->left, 0, 0, 0);
        if (node->right) enqueue(queue, node->right, 0, 0, 0);
        free(current);
    }
    printf("\n");
}